using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;

namespace Zhasyl.Api.Features.Assignments;

public static class GetAssignmentContent
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet(
                "/api/laboratories/{laboratorySlug}/missions/{missionSlug}/assignments/{assignmentSlug}",
                async (
                    string laboratorySlug,
                    string missionSlug,
                    string assignmentSlug,
                    string? locale,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var request = new Request(
                        laboratorySlug,
                        missionSlug,
                        assignmentSlug,
                        (locale ?? "ru").Trim().ToLowerInvariant());
                    var response = await sender.Send(request, cancellationToken);

                    return response is null
                        ? Results.Problem(
                            statusCode: StatusCodes.Status404NotFound,
                            title: "Station assignment not found",
                            detail: "The requested assignment revision is not published.",
                            extensions: new Dictionary<string, object?>
                            {
                                ["code"] = "content:assignment:read:not_found"
                            })
                        : Results.Ok(response);
                });
        }
    }

    public sealed record Request(
        string LaboratorySlug,
        string MissionSlug,
        string AssignmentSlug,
        string Locale) : IRequest<Response?>;

    public sealed record Response(
        string LaboratoryId,
        string LaboratoryName,
        string MissionId,
        string MissionName,
        string AssignmentId,
        Guid RevisionId,
        int Version,
        int Order,
        string Locale,
        string Name,
        string Objective,
        int EstimatedMinutes,
        string BodyMdx);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        private const string SlugPattern = "^[a-z0-9]+(?:-[a-z0-9]+)*$";

        public RequestValidator()
        {
            RuleFor(request => request.LaboratorySlug)
                .Matches(SlugPattern)
                .WithErrorCode("content:laboratory:read:invalid_slug");
            RuleFor(request => request.MissionSlug)
                .Matches(SlugPattern)
                .WithErrorCode("content:mission:read:invalid_slug");
            RuleFor(request => request.AssignmentSlug)
                .Matches(SlugPattern)
                .WithErrorCode("content:assignment:read:invalid_slug");
            RuleFor(request => request.Locale)
                .Matches("^[a-z]{2}(?:-[a-z]{2})?$")
                .WithErrorCode("content:locale:read:invalid");
        }
    }

    public sealed class RequestHandler(AppDbContext db) : IRequestHandler<Request, Response?>
    {
        public async Task<Response?> Handle(Request request, CancellationToken cancellationToken)
        {
            var revision = await db.StationAssignmentRevisions
                .AsNoTracking()
                .Where(item =>
                    item.StationAssignment.Mission.Laboratory.Slug == request.LaboratorySlug &&
                    item.StationAssignment.Mission.Slug == request.MissionSlug &&
                    item.StationAssignment.Slug == request.AssignmentSlug &&
                    item.StationAssignment.Mission.Laboratory.IsPublished &&
                    item.StationAssignment.Mission.IsPublished &&
                    item.StationAssignment.IsPublished &&
                    item.Locale == request.Locale &&
                    item.IsCurrent &&
                    item.PublishedAt != null &&
                    item.StationAssignment.Mission.Revisions.Any(missionRevision =>
                        missionRevision.Locale == request.Locale &&
                        missionRevision.IsCurrent &&
                        missionRevision.PublishedAt != null))
                .Select(item => new
                {
                    LaboratoryId = item.StationAssignment.Mission.Laboratory.Slug,
                    LaboratoryEntityId = item.StationAssignment.Mission.LaboratoryId,
                    MissionId = item.StationAssignment.Mission.Slug,
                    MissionEntityId = item.StationAssignment.MissionId,
                    AssignmentId = item.StationAssignment.Slug,
                    RevisionId = item.Id,
                    item.Version,
                    item.StationAssignment.Order,
                    item.Locale,
                    item.Name,
                    item.Objective,
                    item.EstimatedMinutes,
                    item.BodyMdx,
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (revision is null)
            {
                return null;
            }

            var laboratoryName = await db.LaboratoryTranslations
                .AsNoTracking()
                .Where(item =>
                    item.LaboratoryId == revision.LaboratoryEntityId &&
                    item.Locale == request.Locale)
                .Select(item => item.Name)
                .SingleOrDefaultAsync(cancellationToken);
            var missionName = await db.MissionRevisions
                .AsNoTracking()
                .Where(item =>
                    item.MissionId == revision.MissionEntityId &&
                    item.Locale == request.Locale &&
                    item.IsCurrent &&
                    item.PublishedAt != null)
                .Select(item => item.Name)
                .SingleOrDefaultAsync(cancellationToken);

            return laboratoryName is null || missionName is null
                ? null
                : new Response(
                    revision.LaboratoryId,
                    laboratoryName,
                    revision.MissionId,
                    missionName,
                    revision.AssignmentId,
                    revision.RevisionId,
                    revision.Version,
                    revision.Order,
                    revision.Locale,
                    revision.Name,
                    revision.Objective,
                    revision.EstimatedMinutes,
                    revision.BodyMdx);
        }
    }
}
