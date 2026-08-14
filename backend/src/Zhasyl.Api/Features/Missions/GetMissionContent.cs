using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;

namespace Zhasyl.Api.Features.Missions;

public static class GetMissionContent
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet(
                "/api/laboratories/{laboratorySlug}/missions/{missionSlug}",
                async (
                    string laboratorySlug,
                    string missionSlug,
                    string? locale,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var request = new Request(
                        laboratorySlug,
                        missionSlug,
                        (locale ?? "ru").Trim().ToLowerInvariant());
                    var response = await sender.Send(request, cancellationToken);

                    return response is null
                        ? Results.Problem(
                            statusCode: StatusCodes.Status404NotFound,
                            title: "Mission not found",
                            detail: "The requested mission revision is not published.",
                            extensions: new Dictionary<string, object?>
                            {
                                ["code"] = "content:mission:read:not_found"
                            })
                        : Results.Ok(response);
                });
        }
    }

    public sealed record Request(
        string LaboratorySlug,
        string MissionSlug,
        string Locale) : IRequest<Response?>;

    public sealed record Response(
        string LaboratoryId,
        string LaboratoryName,
        string MissionId,
        Guid RevisionId,
        int Version,
        string Locale,
        string Name,
        string Problem,
        string Status,
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
            RuleFor(request => request.Locale)
                .Matches("^[a-z]{2}(?:-[a-z]{2})?$")
                .WithErrorCode("content:locale:read:invalid");
        }
    }

    public sealed class RequestHandler(AppDbContext db) : IRequestHandler<Request, Response?>
    {
        public async Task<Response?> Handle(Request request, CancellationToken cancellationToken)
        {
            var revision = await db.MissionRevisions
                .AsNoTracking()
                .Where(item =>
                    item.Mission.Laboratory.Slug == request.LaboratorySlug &&
                    item.Mission.Slug == request.MissionSlug &&
                    item.Mission.Laboratory.IsPublished &&
                    item.Mission.IsPublished &&
                    item.Locale == request.Locale &&
                    item.IsCurrent &&
                    item.PublishedAt != null)
                .Select(item => new
                {
                    LaboratoryKey = item.Mission.Laboratory.Slug,
                    LaboratoryEntityId = item.Mission.LaboratoryId,
                    MissionId = item.Mission.Slug,
                    RevisionId = item.Id,
                    item.Version,
                    item.Locale,
                    item.Name,
                    item.Problem,
                    item.Status,
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

            return laboratoryName is null
                ? null
                : new Response(
                    revision.LaboratoryKey,
                    laboratoryName,
                    revision.MissionId,
                    revision.RevisionId,
                    revision.Version,
                    revision.Locale,
                    revision.Name,
                    revision.Problem,
                    revision.Status,
                    revision.BodyMdx);
        }
    }
}
