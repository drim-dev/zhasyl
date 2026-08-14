using System.Net;
using System.Net.Http.Json;
using Zhasyl.Api.Features.Assignments;
using Zhasyl.Api.Tests.Infrastructure;

namespace Zhasyl.Api.Tests.Features.Assignments;

public sealed class GetAssignmentContentTests : IClassFixture<ZhasylApplicationFactory>
{
    private readonly HttpClient client;

    public GetAssignmentContentTests(ZhasylApplicationFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_return_the_current_published_assignment_revision()
    {
        var response = await client.GetAsync(
            "/api/laboratories/bioinformatics/missions/bioscout/assignments/check-sequence?locale=ru");
        var assignment = await response.Content.ReadFromJsonAsync<GetAssignmentContent.Response>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(assignment);
        Assert.Equal("bioinformatics", assignment.LaboratoryId);
        Assert.Equal("bioscout", assignment.MissionId);
        Assert.Equal("check-sequence", assignment.AssignmentId);
        Assert.Equal(1, assignment.Version);
        Assert.Equal(60, assignment.EstimatedMinutes);
        Assert.NotEqual(Guid.Empty, assignment.RevisionId);
        Assert.Contains("SequenceInspector", assignment.BodyMdx);
    }

    [Fact]
    public async Task Should_return_not_found_for_an_unknown_assignment()
    {
        var response = await client.GetAsync(
            "/api/laboratories/bioinformatics/missions/bioscout/assignments/missing?locale=ru");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Should_reject_an_invalid_assignment_slug()
    {
        var response = await client.GetAsync(
            "/api/laboratories/bioinformatics/missions/bioscout/assignments/NOT_VALID?locale=ru");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
