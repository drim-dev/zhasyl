using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;

namespace Zhasyl.Api.Features.Station;

public static class GetStationOverview
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/station/overview", async (
                string? locale,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var normalizedLocale = (locale ?? "ru").Trim().ToLowerInvariant();
                var response = await sender.Send(new Request(normalizedLocale), cancellationToken);

                return response is null
                    ? Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Content not found",
                        detail: "The requested station content is not published in this locale.",
                        extensions: new Dictionary<string, object?>
                        {
                            ["code"] = "content:locale:read:not_published"
                        })
                    : Results.Ok(response);
            });
        }
    }

    public sealed record Request(string Locale) : IRequest<Response?>;

    public sealed record Response(
        string StationId,
        string StationName,
        string Locale,
        string Location,
        string Briefing,
        IReadOnlyList<LaboratorySummary> Laboratories);

    public sealed record LaboratorySummary(
        string Id,
        string Name,
        string Purpose,
        string Specialist,
        MissionSummary FirstMission);

    public sealed record MissionSummary(
        string Id,
        string Name,
        string Problem,
        string Status);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(request => request.Locale)
                .Matches("^[a-z]{2}(?:-[a-z]{2})?$")
                .WithMessage("The locale must be a supported language tag.")
                .WithErrorCode("content:locale:read:invalid");
        }
    }

    public sealed class RequestHandler(AppDbContext db) : IRequestHandler<Request, Response?>
    {
        public async Task<Response?> Handle(Request request, CancellationToken cancellationToken)
        {
            var station = await db.StationTranslations
                .AsNoTracking()
                .Where(translation =>
                    translation.Station.Slug == "zhasyl-1" &&
                    translation.Locale == request.Locale)
                .Select(translation => new
                {
                    translation.StationId,
                    translation.Station.Slug,
                    translation.Name,
                    translation.Location,
                    translation.Briefing,
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (station is null)
            {
                return null;
            }

            var laboratoryRows = await db.LaboratoryTranslations
                .AsNoTracking()
                .Where(translation =>
                    translation.Laboratory.StationId == station.StationId &&
                    translation.Laboratory.IsPublished &&
                    translation.Locale == request.Locale)
                .OrderBy(translation => translation.Laboratory.Order)
                .Select(translation => new
                {
                    translation.LaboratoryId,
                    translation.Laboratory.Slug,
                    translation.Name,
                    translation.Purpose,
                    translation.Specialist,
                })
                .ToListAsync(cancellationToken);

            var missionRows = await db.MissionRevisions
                .AsNoTracking()
                .Where(revision =>
                    revision.Mission.Laboratory.StationId == station.StationId &&
                    revision.Mission.Laboratory.IsPublished &&
                    revision.Mission.IsPublished &&
                    revision.Locale == request.Locale &&
                    revision.IsCurrent &&
                    revision.PublishedAt != null)
                .OrderBy(revision => revision.Mission.Laboratory.Order)
                .ThenBy(revision => revision.Mission.Order)
                .Select(revision => new
                {
                    revision.Mission.LaboratoryId,
                    Summary = new MissionSummary(
                        revision.Mission.Slug,
                        revision.Name,
                        revision.Problem,
                        revision.Status),
                })
                .ToListAsync(cancellationToken);

            var firstMissionByLaboratory = missionRows
                .GroupBy(row => row.LaboratoryId)
                .ToDictionary(group => group.Key, group => group.First().Summary);
            var laboratories = laboratoryRows
                .Where(row => firstMissionByLaboratory.ContainsKey(row.LaboratoryId))
                .Select(row => new LaboratorySummary(
                    row.Slug,
                    row.Name,
                    row.Purpose,
                    row.Specialist,
                    firstMissionByLaboratory[row.LaboratoryId]))
                .ToList();

            return new Response(
                station.Slug,
                station.Name,
                request.Locale,
                station.Location,
                station.Briefing,
                laboratories);
        }
    }
}
