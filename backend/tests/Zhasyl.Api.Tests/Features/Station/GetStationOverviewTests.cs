using System.Net;
using System.Net.Http.Json;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc.Testing;
using Zhasyl.Api.Features.Station;

namespace Zhasyl.Api.Tests.Features.Station;

public sealed class GetStationOverviewTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public GetStationOverviewTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_return_published_station_overview()
    {
        var response = await client.GetAsync("/api/station/overview?locale=ru");
        var overview = await response.Content.ReadFromJsonAsync<GetStationOverview.Response>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(overview);
        Assert.Equal("zhasyl-1", overview.StationId);
        Assert.Equal("ru", overview.Locale);
        Assert.Collection(
            overview.Laboratories,
            laboratory => Assert.Equal("bioinformatics", laboratory.Id),
            laboratory => Assert.Equal("materials", laboratory.Id));
    }

    public sealed class ValidatorTests
    {
        private readonly GetStationOverview.RequestValidator validator = new();

        [Fact]
        public void Should_accept_published_locale()
        {
            var result = validator.TestValidate(new GetStationOverview.Request("ru"));

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_reject_unpublished_locale()
        {
            var result = validator.TestValidate(new GetStationOverview.Request("kk"));

            result.ShouldHaveValidationErrorFor(request => request.Locale)
                .WithErrorCode("content:locale:read:not_published");
        }
    }
}
