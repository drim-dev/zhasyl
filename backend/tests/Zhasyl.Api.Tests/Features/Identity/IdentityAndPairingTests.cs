using System.Net;
using System.Net.Http.Json;
using FluentValidation.TestHelper;
using Zhasyl.Api.Features.Children;
using Zhasyl.Api.Features.Identity;
using Zhasyl.Api.Features.Pairing;
using Zhasyl.Api.Tests.Infrastructure;

namespace Zhasyl.Api.Tests.Features.Identity;

public sealed class IdentityAndPairingTests : IClassFixture<ZhasylApplicationFactory>
{
    private readonly ZhasylApplicationFactory factory;

    public IdentityAndPairingTests(ZhasylApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Should_pair_and_revoke_a_child_device()
    {
        var (adultId, email) = await SignInAdult($"parent-{Guid.NewGuid():N}@example.com");
        using var adultClient = CreateAdultClient(adultId, email);
        var childResponse = await adultClient.PostAsJsonAsync(
            "/api/adult/children",
            new { displayName = "Лиза", learningLocale = "ru" });
        var child = await childResponse.Content.ReadFromJsonAsync<CreateChildProfile.Response>();

        Assert.Equal(HttpStatusCode.Created, childResponse.StatusCode);
        Assert.NotNull(child);

        var codeResponse = await adultClient.PostAsync(
            $"/api/adult/children/{child.ChildId}/pairing-codes",
            null);
        var code = await codeResponse.Content.ReadFromJsonAsync<CreatePairingCode.Response>();
        Assert.Equal(HttpStatusCode.OK, codeResponse.StatusCode);
        Assert.NotNull(code);

        using var anonymousClient = factory.CreateClient();
        var pairResponse = await anonymousClient.PostAsJsonAsync(
            "/api/child/pair",
            new { code = code.Code, deviceName = "Домашний компьютер" });
        var session = await pairResponse.Content.ReadFromJsonAsync<RedeemPairingCode.Response>();
        Assert.Equal(HttpStatusCode.OK, pairResponse.StatusCode);
        Assert.NotNull(session);
        Assert.Equal(child.ChildId, session.ChildId);

        using var childClient = factory.CreateClient();
        childClient.DefaultRequestHeaders.Add("X-Child-Session", session.SessionToken);
        var currentResponse = await childClient.GetAsync("/api/child/session");
        var current = await currentResponse.Content.ReadFromJsonAsync<GetChildSession.Response>();
        Assert.Equal(HttpStatusCode.OK, currentResponse.StatusCode);
        Assert.Equal("Лиза", current?.DisplayName);

        var children = await adultClient.GetFromJsonAsync<ListChildProfiles.Response>(
            "/api/adult/children");
        var device = Assert.Single(Assert.Single(children!.Children).Devices);
        Assert.False(device.IsRevoked);

        var revokeResponse = await adultClient.DeleteAsync(
            $"/api/adult/children/{child.ChildId}/devices/{device.DeviceId}");
        Assert.Equal(HttpStatusCode.NoContent, revokeResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await childClient.GetAsync("/api/child/session")).StatusCode);
    }

    [Fact]
    public async Task Should_not_accept_a_pairing_code_twice()
    {
        var (adultId, email) = await SignInAdult($"parent-{Guid.NewGuid():N}@example.com");
        using var adultClient = CreateAdultClient(adultId, email);
        var child = await (await adultClient.PostAsJsonAsync(
                "/api/adult/children",
                new { displayName = "Илья", learningLocale = "ru" }))
            .Content.ReadFromJsonAsync<CreateChildProfile.Response>();
        var code = await (await adultClient.PostAsync(
                $"/api/adult/children/{child!.ChildId}/pairing-codes",
                null))
            .Content.ReadFromJsonAsync<CreatePairingCode.Response>();
        using var anonymousClient = factory.CreateClient();
        var request = new { code = code!.Code, deviceName = "Ноутбук" };

        Assert.Equal(HttpStatusCode.OK, (await anonymousClient.PostAsJsonAsync("/api/child/pair", request)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await anonymousClient.PostAsJsonAsync("/api/child/pair", request)).StatusCode);
    }

    [Fact]
    public async Task Should_require_an_adult_session_for_child_management()
    {
        using var client = factory.CreateClient();

        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/adult/children")).StatusCode);
    }

    [Fact]
    public async Task Should_reuse_an_existing_oauth_identity()
    {
        using var client = factory.CreateClient();
        var providerUserId = Guid.NewGuid().ToString("N");
        var body = new
        {
            provider = "development",
            providerUserId,
            providerEmail = $"parent-{Guid.NewGuid():N}@example.com",
        };

        var first = await (await client.PostAsJsonAsync("/api/auth/oauth-sign-in", body))
            .Content.ReadFromJsonAsync<HandleOAuthSignIn.Response>();
        var second = await (await client.PostAsJsonAsync("/api/auth/oauth-sign-in", body))
            .Content.ReadFromJsonAsync<HandleOAuthSignIn.Response>();

        Assert.Equal(first?.AdultId, second?.AdultId);
    }

    [Fact]
    public async Task Should_not_link_a_second_provider_only_by_matching_email()
    {
        using var client = factory.CreateClient();
        var email = $"parent-{Guid.NewGuid():N}@example.com";
        var first = await client.PostAsJsonAsync(
            "/api/auth/oauth-sign-in",
            new
            {
                provider = "google",
                providerUserId = Guid.NewGuid().ToString("N"),
                providerEmail = email,
            });
        var second = await client.PostAsJsonAsync(
            "/api/auth/oauth-sign-in",
            new
            {
                provider = "github",
                providerUserId = Guid.NewGuid().ToString("N"),
                providerEmail = email,
            });

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    private async Task<(Guid AdultId, string Email)> SignInAdult(string email)
    {
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/auth/oauth-sign-in",
            new
            {
                provider = "development",
                providerUserId = Guid.NewGuid().ToString("N"),
                providerEmail = email,
            });
        response.EnsureSuccessStatusCode();
        var adult = await response.Content.ReadFromJsonAsync<HandleOAuthSignIn.Response>();
        return (adult!.AdultId, adult.Email);
    }

    private HttpClient CreateAdultClient(Guid adultId, string email)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Adult-Id", adultId.ToString());
        client.DefaultRequestHeaders.Add("X-Adult-Email", email);
        return client;
    }

    public sealed class OAuthValidatorTests
    {
        private readonly HandleOAuthSignIn.RequestValidator validator = new();

        [Fact]
        public void Should_validate_provider_identity_and_email()
        {
            var result = validator.TestValidate(new HandleOAuthSignIn.Request("unknown", "", "bad"));

            result.ShouldHaveValidationErrorFor(request => request.Provider);
            result.ShouldHaveValidationErrorFor(request => request.ProviderUserId);
            result.ShouldHaveValidationErrorFor(request => request.ProviderEmail);
        }
    }

    public sealed class ChildProfileValidatorTests
    {
        private readonly CreateChildProfile.RequestValidator validator = new();

        [Fact]
        public void Should_accept_supported_learning_locales()
        {
            validator.TestValidate(new CreateChildProfile.Request(Guid.NewGuid(), "Лиза", "ru"))
                .ShouldNotHaveAnyValidationErrors();
            validator.TestValidate(new CreateChildProfile.Request(Guid.NewGuid(), "Алия", "kk"))
                .ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_reject_an_empty_name_and_unknown_locale()
        {
            var result = validator.TestValidate(new CreateChildProfile.Request(Guid.NewGuid(), "", "en"));

            result.ShouldHaveValidationErrorFor(request => request.DisplayName);
            result.ShouldHaveValidationErrorFor(request => request.LearningLocale);
        }
    }

    public sealed class PairingValidatorTests
    {
        private readonly RedeemPairingCode.RequestValidator validator = new();

        [Fact]
        public void Should_accept_a_formatted_code_and_device_name()
        {
            validator.TestValidate(new RedeemPairingCode.Request("2345-6789", "Домашний компьютер"))
                .ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_reject_an_invalid_code_and_empty_device_name()
        {
            var result = validator.TestValidate(new RedeemPairingCode.Request("123", ""));

            result.ShouldHaveValidationErrorFor(request => request.Code);
            result.ShouldHaveValidationErrorFor(request => request.DeviceName);
        }
    }
}
