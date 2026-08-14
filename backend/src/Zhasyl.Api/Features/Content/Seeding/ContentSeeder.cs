using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Database;
using Zhasyl.Api.Domain.Content;
using ContentStation = Zhasyl.Api.Domain.Content.Station;

namespace Zhasyl.Api.Features.Content.Seeding;

public sealed class ContentSeeder(
    AppDbContext db,
    IContentSeedSource source,
    TimeProvider timeProvider,
    ILogger<ContentSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var documents = await source.LoadAsync(cancellationToken);

        await SeedStationsAsync(documents, cancellationToken);
        await SeedLaboratoriesAsync(documents, cancellationToken);
        await SeedMissionsAsync(documents, cancellationToken);
        await SeedAssignmentsAsync(documents, cancellationToken);

        logger.LogInformation("Seeded {Count} content documents.", documents.Count);
    }

    private async Task SeedStationsAsync(
        IReadOnlyList<ContentSeedDocument> documents,
        CancellationToken cancellationToken)
    {
        foreach (var document in documents.Where(document => document.Kind == ContentKind.Station))
        {
            var station = await db.Stations
                .Include(item => item.Translations)
                .SingleOrDefaultAsync(item => item.Slug == document.Slug, cancellationToken);

            if (station is null)
            {
                station = new ContentStation { Id = Guid.CreateVersion7(), Slug = document.Slug };
                db.Stations.Add(station);
            }

            var translation = station.Translations
                .SingleOrDefault(item => item.Locale == document.Locale);
            if (translation is null)
            {
                translation = new StationTranslation
                {
                    StationId = station.Id,
                    Locale = document.Locale,
                    Name = document.Title,
                    Location = document.Location!,
                    Briefing = document.Briefing!,
                };
                station.Translations.Add(translation);
            }
            else
            {
                translation.Name = document.Title;
                translation.Location = document.Location!;
                translation.Briefing = document.Briefing!;
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedLaboratoriesAsync(
        IReadOnlyList<ContentSeedDocument> documents,
        CancellationToken cancellationToken)
    {
        foreach (var document in documents.Where(document => document.Kind == ContentKind.Laboratory))
        {
            var station = await db.Stations.SingleOrDefaultAsync(
                item => item.Slug == document.Station,
                cancellationToken) ?? throw MissingParent(document, "station", document.Station);

            var laboratory = await db.Laboratories
                .Include(item => item.Translations)
                .SingleOrDefaultAsync(
                    item => item.StationId == station.Id && item.Slug == document.Slug,
                    cancellationToken);

            if (laboratory is null)
            {
                laboratory = new Laboratory
                {
                    Id = Guid.CreateVersion7(),
                    StationId = station.Id,
                    Slug = document.Slug,
                    Order = document.Order,
                    IsPublished = document.IsPublished,
                };
                db.Laboratories.Add(laboratory);
            }
            else
            {
                laboratory.Order = document.Order;
                laboratory.IsPublished = document.IsPublished;
            }

            var translation = laboratory.Translations
                .SingleOrDefault(item => item.Locale == document.Locale);
            if (translation is null)
            {
                translation = new LaboratoryTranslation
                {
                    LaboratoryId = laboratory.Id,
                    Locale = document.Locale,
                    Name = document.Title,
                    Purpose = document.Purpose!,
                    Specialist = document.Specialist!,
                };
                laboratory.Translations.Add(translation);
            }
            else
            {
                translation.Name = document.Title;
                translation.Purpose = document.Purpose!;
                translation.Specialist = document.Specialist!;
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedMissionsAsync(
        IReadOnlyList<ContentSeedDocument> documents,
        CancellationToken cancellationToken)
    {
        var missionDocuments = documents
            .Where(document => document.Kind == ContentKind.Mission)
            .ToArray();

        foreach (var document in missionDocuments)
        {
            var laboratory = await db.Laboratories.SingleOrDefaultAsync(
                item => item.Slug == document.Laboratory,
                cancellationToken) ?? throw MissingParent(document, "laboratory", document.Laboratory);

            var mission = await db.Missions.SingleOrDefaultAsync(
                item => item.LaboratoryId == laboratory.Id && item.Slug == document.Slug,
                cancellationToken);

            if (mission is null)
            {
                mission = new Mission
                {
                    Id = Guid.CreateVersion7(),
                    LaboratoryId = laboratory.Id,
                    Slug = document.Slug,
                    Order = document.Order,
                    IsPublished = document.IsPublished,
                };
                db.Missions.Add(mission);
            }
            else
            {
                mission.Order = document.Order;
                mission.IsPublished = document.IsPublished;
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        foreach (var document in missionDocuments)
        {
            var laboratory = await db.Laboratories.SingleAsync(
                item => item.Slug == document.Laboratory,
                cancellationToken);
            var mission = await db.Missions.SingleAsync(
                item => item.LaboratoryId == laboratory.Id && item.Slug == document.Slug,
                cancellationToken);
            var revisions = await db.MissionRevisions
                .Where(item => item.MissionId == mission.Id && item.Locale == document.Locale)
                .ToListAsync(cancellationToken);
            var current = revisions.SingleOrDefault(item => item.IsCurrent);

            if (current?.ContentHash == document.ContentHash)
            {
                continue;
            }

            if (current is not null)
            {
                current.IsCurrent = false;
            }

            var now = timeProvider.GetUtcNow();
            db.MissionRevisions.Add(new MissionRevision
            {
                Id = Guid.CreateVersion7(),
                MissionId = mission.Id,
                Locale = document.Locale,
                Version = revisions.Select(item => item.Version).DefaultIfEmpty().Max() + 1,
                Name = document.Title,
                Problem = document.Problem!,
                Status = document.Status!,
                BodyMdx = document.BodyMdx,
                ContentHash = document.ContentHash,
                IsCurrent = true,
                CreatedAt = now,
                PublishedAt = document.IsPublished ? now : null,
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedAssignmentsAsync(
        IReadOnlyList<ContentSeedDocument> documents,
        CancellationToken cancellationToken)
    {
        var assignmentDocuments = documents
            .Where(document => document.Kind == ContentKind.Assignment)
            .ToArray();

        foreach (var document in assignmentDocuments)
        {
            var mission = await db.Missions.SingleOrDefaultAsync(
                item =>
                    item.Laboratory.Slug == document.Laboratory &&
                    item.Slug == document.Mission,
                cancellationToken) ?? throw MissingParent(document, "mission", document.Mission);

            var assignment = await db.StationAssignments.SingleOrDefaultAsync(
                item => item.MissionId == mission.Id && item.Slug == document.Slug,
                cancellationToken);

            if (assignment is null)
            {
                assignment = new StationAssignment
                {
                    Id = Guid.CreateVersion7(),
                    MissionId = mission.Id,
                    Slug = document.Slug,
                    Order = document.Order,
                    IsPublished = document.IsPublished,
                };
                db.StationAssignments.Add(assignment);
            }
            else
            {
                assignment.Order = document.Order;
                assignment.IsPublished = document.IsPublished;
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        foreach (var document in assignmentDocuments)
        {
            var assignment = await db.StationAssignments.SingleAsync(
                item =>
                    item.Mission.Laboratory.Slug == document.Laboratory &&
                    item.Mission.Slug == document.Mission &&
                    item.Slug == document.Slug,
                cancellationToken);
            var revisions = await db.StationAssignmentRevisions
                .Where(item =>
                    item.StationAssignmentId == assignment.Id &&
                    item.Locale == document.Locale)
                .ToListAsync(cancellationToken);
            var current = revisions.SingleOrDefault(item => item.IsCurrent);

            if (current?.ContentHash == document.ContentHash)
            {
                continue;
            }

            if (current is not null)
            {
                current.IsCurrent = false;
            }

            var now = timeProvider.GetUtcNow();
            db.StationAssignmentRevisions.Add(new StationAssignmentRevision
            {
                Id = Guid.CreateVersion7(),
                StationAssignmentId = assignment.Id,
                Locale = document.Locale,
                Version = revisions.Select(item => item.Version).DefaultIfEmpty().Max() + 1,
                Name = document.Title,
                Objective = document.Objective!,
                EstimatedMinutes = document.EstimatedMinutes,
                BodyMdx = document.BodyMdx,
                ContentHash = document.ContentHash,
                IsCurrent = true,
                CreatedAt = now,
                PublishedAt = document.IsPublished ? now : null,
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static InvalidDataException MissingParent(
        ContentSeedDocument document,
        string parentType,
        string? parentSlug) => new(
            $"Content file '{document.SourceLocation}' references missing {parentType} '{parentSlug}'.");
}
