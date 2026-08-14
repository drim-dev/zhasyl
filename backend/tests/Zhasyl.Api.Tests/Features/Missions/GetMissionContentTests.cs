using System.Net;
using System.Net.Http.Json;
using Zhasyl.Api.Features.Missions;
using Zhasyl.Api.Tests.Infrastructure;

namespace Zhasyl.Api.Tests.Features.Missions;

public sealed class GetMissionContentTests : IClassFixture<ZhasylApplicationFactory>
{
    private readonly HttpClient client;

    public GetMissionContentTests(ZhasylApplicationFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_return_the_current_published_mdx_revision()
    {
        var response = await client.GetAsync(
            "/api/laboratories/bioinformatics/missions/bioscout?locale=ru");
        var mission = await response.Content.ReadFromJsonAsync<GetMissionContent.Response>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(mission);
        Assert.Equal("bioinformatics", mission.LaboratoryId);
        Assert.Equal("bioscout", mission.MissionId);
        Assert.Equal(1, mission.Version);
        Assert.NotEqual(Guid.Empty, mission.RevisionId);
        Assert.Contains("формате FASTA", mission.BodyMdx);
    }

    [Fact]
    public async Task Should_return_not_found_for_an_unpublished_locale()
    {
        var response = await client.GetAsync(
            "/api/laboratories/bioinformatics/missions/bioscout?locale=kk");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
