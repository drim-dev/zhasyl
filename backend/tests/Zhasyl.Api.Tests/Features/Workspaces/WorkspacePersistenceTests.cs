using System.Net;
using System.Net.Http.Json;
using FluentValidation.TestHelper;
using Zhasyl.Api.Features.Assignments;
using Zhasyl.Api.Features.Children;
using Zhasyl.Api.Features.Identity;
using Zhasyl.Api.Features.Pairing;
using Zhasyl.Api.Features.Workspaces;
using Zhasyl.Api.Tests.Infrastructure;

namespace Zhasyl.Api.Tests.Features.Workspaces;

public sealed class WorkspacePersistenceTests : IClassFixture<ZhasylApplicationFactory>
{
    private readonly ZhasylApplicationFactory factory;

    public WorkspacePersistenceTests(ZhasylApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Should_save_restore_version_and_reject_a_stale_write()
    {
        using var childClient = await CreatePairedChildClient();
        var revisionId = await GetAssignmentRevisionId();

        var empty = await childClient.GetFromJsonAsync<GetWorkspace.Response>(
            $"/api/child/workspaces/{revisionId}");
        Assert.Equal(0, empty?.Version);
        Assert.Null(empty?.Code);

        var firstResponse = await childClient.PutAsJsonAsync(
            $"/api/child/workspaces/{revisionId}",
            new { expectedVersion = 0, code = "print('first')" });
        var first = await firstResponse.Content.ReadFromJsonAsync<SaveWorkspace.Response>();
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(1, first?.Version);

        var staleResponse = await childClient.PutAsJsonAsync(
            $"/api/child/workspaces/{revisionId}",
            new { expectedVersion = 0, code = "print('stale')" });
        Assert.Equal(HttpStatusCode.Conflict, staleResponse.StatusCode);

        var secondResponse = await childClient.PutAsJsonAsync(
            $"/api/child/workspaces/{revisionId}",
            new { expectedVersion = 1, code = "print('second')" });
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);

        var restored = await childClient.GetFromJsonAsync<GetWorkspace.Response>(
            $"/api/child/workspaces/{revisionId}");
        Assert.Equal(2, restored?.Version);
        Assert.Equal("print('second')", restored?.Code);
        Assert.NotNull(restored?.SavedAt);
    }

    [Fact]
    public async Task Should_isolate_workspaces_by_child_profile()
    {
        var revisionId = await GetAssignmentRevisionId();
        using var firstChild = await CreatePairedChildClient();
        using var secondChild = await CreatePairedChildClient();

        await firstChild.PutAsJsonAsync(
            $"/api/child/workspaces/{revisionId}",
            new { expectedVersion = 0, code = "private child work" });
        var secondWorkspace = await secondChild.GetFromJsonAsync<GetWorkspace.Response>(
            $"/api/child/workspaces/{revisionId}");

        Assert.Equal(0, secondWorkspace?.Version);
        Assert.Null(secondWorkspace?.Code);
    }

    [Fact]
    public async Task Should_require_a_child_device_session()
    {
        using var client = factory.CreateClient();
        var revisionId = await GetAssignmentRevisionId();

        var response = await client.GetAsync($"/api/child/workspaces/{revisionId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<Guid> GetAssignmentRevisionId()
    {
        using var client = factory.CreateClient();
        var assignment = await client.GetFromJsonAsync<GetAssignmentContent.Response>(
            "/api/laboratories/bioinformatics/missions/bioscout/assignments/check-sequence?locale=ru");
        return assignment!.RevisionId;
    }

    private async Task<HttpClient> CreatePairedChildClient()
    {
        using var anonymousClient = factory.CreateClient();
        var adultResponse = await anonymousClient.PostAsJsonAsync(
            "/api/auth/oauth-sign-in",
            new
            {
                provider = "development",
                providerUserId = Guid.NewGuid().ToString("N"),
                providerEmail = $"parent-{Guid.NewGuid():N}@example.com",
            });
        var adult = await adultResponse.Content.ReadFromJsonAsync<HandleOAuthSignIn.Response>();

        using var adultClient = factory.CreateClient();
        adultClient.DefaultRequestHeaders.Add("X-Adult-Id", adult!.AdultId.ToString());
        adultClient.DefaultRequestHeaders.Add("X-Adult-Email", adult.Email);
        var child = await (await adultClient.PostAsJsonAsync(
                "/api/adult/children",
                new { displayName = "Ребёнок", learningLocale = "ru" }))
            .Content.ReadFromJsonAsync<CreateChildProfile.Response>();
        var code = await (await adultClient.PostAsync(
                $"/api/adult/children/{child!.ChildId}/pairing-codes",
                null))
            .Content.ReadFromJsonAsync<CreatePairingCode.Response>();
        var session = await (await anonymousClient.PostAsJsonAsync(
                "/api/child/pair",
                new { code = code!.Code, deviceName = "Тестовый браузер" }))
            .Content.ReadFromJsonAsync<RedeemPairingCode.Response>();

        var childClient = factory.CreateClient();
        childClient.DefaultRequestHeaders.Add("X-Child-Session", session!.SessionToken);
        return childClient;
    }

    public sealed class ValidatorTests
    {
        private readonly SaveWorkspace.RequestValidator validator = new();

        [Fact]
        public void Should_reject_a_negative_version_and_oversized_code()
        {
            var result = validator.TestValidate(new SaveWorkspace.Request(
                Guid.NewGuid(),
                Guid.NewGuid(),
                -1,
                new string('ж', 100_001)));

            result.ShouldHaveValidationErrorFor(request => request.ExpectedVersion);
            result.ShouldHaveValidationErrorFor(request => request.Code);
        }

        [Fact]
        public void Should_reject_a_null_code_without_throwing()
        {
            var result = validator.TestValidate(new SaveWorkspace.Request(
                Guid.NewGuid(),
                Guid.NewGuid(),
                0,
                null!));

            result.ShouldHaveValidationErrorFor(request => request.Code);
        }
    }
}
