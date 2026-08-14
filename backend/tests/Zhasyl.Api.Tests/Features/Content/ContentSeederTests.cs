using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Zhasyl.Api.Database;
using Zhasyl.Api.Features.Content.Seeding;

namespace Zhasyl.Api.Tests.Features.Content;

public sealed class ContentSeederTests
{
    [Fact]
    public async Task Should_create_a_new_revision_only_when_mission_content_changes()
    {
        await using var db = CreateDatabase();
        var source = new MutableSeedSource(CreateDocuments(
            missionHash: "mission-v1",
            missionBody: "Первая версия миссии",
            assignmentHash: "assignment-v1",
            assignmentBody: "Первая версия задания"));
        var seeder = CreateSeeder(db, source);

        await seeder.SeedAsync(CancellationToken.None);
        await seeder.SeedAsync(CancellationToken.None);

        Assert.Single(await db.MissionRevisions.ToListAsync());

        source.Documents = CreateDocuments(
            missionHash: "mission-v2",
            missionBody: "Вторая версия миссии",
            assignmentHash: "assignment-v1",
            assignmentBody: "Первая версия задания");
        await seeder.SeedAsync(CancellationToken.None);

        var revisions = await db.MissionRevisions
            .OrderBy(revision => revision.Version)
            .ToListAsync();
        Assert.Collection(
            revisions,
            revision =>
            {
                Assert.Equal(1, revision.Version);
                Assert.False(revision.IsCurrent);
                Assert.Equal("Первая версия миссии", revision.BodyMdx);
            },
            revision =>
            {
                Assert.Equal(2, revision.Version);
                Assert.True(revision.IsCurrent);
                Assert.Equal("Вторая версия миссии", revision.BodyMdx);
            });
        Assert.Single(await db.StationAssignmentRevisions.ToListAsync());
    }

    [Fact]
    public async Task Should_version_assignments_independently_from_their_mission()
    {
        await using var db = CreateDatabase();
        var source = new MutableSeedSource(CreateDocuments(
            missionHash: "mission-v1",
            missionBody: "Версия миссии",
            assignmentHash: "assignment-v1",
            assignmentBody: "Первая версия задания"));
        var seeder = CreateSeeder(db, source);

        await seeder.SeedAsync(CancellationToken.None);
        source.Documents = CreateDocuments(
            missionHash: "mission-v1",
            missionBody: "Версия миссии",
            assignmentHash: "assignment-v2",
            assignmentBody: "Вторая версия задания");
        await seeder.SeedAsync(CancellationToken.None);

        var revisions = await db.StationAssignmentRevisions
            .OrderBy(revision => revision.Version)
            .ToListAsync();
        Assert.Collection(
            revisions,
            revision =>
            {
                Assert.Equal(1, revision.Version);
                Assert.False(revision.IsCurrent);
            },
            revision =>
            {
                Assert.Equal(2, revision.Version);
                Assert.True(revision.IsCurrent);
                Assert.Equal("Вторая версия задания", revision.BodyMdx);
                Assert.Equal(45, revision.EstimatedMinutes);
            });
        Assert.Single(await db.MissionRevisions.ToListAsync());
    }

    private static AppDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"content-seeder-{Guid.NewGuid():N}")
            .Options;
        return new AppDbContext(options);
    }

    private static ContentSeeder CreateSeeder(AppDbContext db, IContentSeedSource source) => new(
        db,
        source,
        TimeProvider.System,
        NullLogger<ContentSeeder>.Instance);

    private static IReadOnlyList<ContentSeedDocument> CreateDocuments(
        string missionHash,
        string missionBody,
        string assignmentHash,
        string assignmentBody) =>
    [
        new(
            ContentKind.Station,
            "zhasyl-1",
            "ru",
            null,
            null,
            null,
            0,
            "Станция «Жасыл-1»",
            "Марс",
            "Подготовка станции",
            null,
            null,
            null,
            null,
            null,
            0,
            true,
            string.Empty,
            "station-hash",
            "ru/station/overview.mdx"),
        new(
            ContentKind.Laboratory,
            "bioinformatics",
            "ru",
            "zhasyl-1",
            null,
            null,
            1,
            "Лаборатория биоинформатики",
            null,
            null,
            "Анализирует живые системы",
            "Лариса Ким",
            null,
            null,
            null,
            0,
            true,
            string.Empty,
            "laboratory-hash",
            "ru/laboratories/bioinformatics/overview.mdx"),
        new(
            ContentKind.Mission,
            "bioscout",
            "ru",
            null,
            "bioinformatics",
            null,
            1,
            "BioScout",
            null,
            null,
            null,
            null,
            "Болезнь растений",
            "Подготовка",
            null,
            0,
            true,
            missionBody,
            missionHash,
            "ru/laboratories/bioinformatics/missions/bioscout/overview.mdx"),
        new(
            ContentKind.Assignment,
            "check-sequence",
            "ru",
            null,
            "bioinformatics",
            "bioscout",
            1,
            "Проверь сигнал",
            null,
            null,
            null,
            null,
            null,
            null,
            "Найти ошибочные символы в последовательности",
            45,
            true,
            assignmentBody,
            assignmentHash,
            "ru/laboratories/bioinformatics/missions/bioscout/assignments/01-check-sequence.mdx"),
    ];

    private sealed class MutableSeedSource(IReadOnlyList<ContentSeedDocument> documents)
        : IContentSeedSource
    {
        public IReadOnlyList<ContentSeedDocument> Documents { get; set; } = documents;

        public Task<IReadOnlyList<ContentSeedDocument>> LoadAsync(
            CancellationToken cancellationToken) => Task.FromResult(Documents);
    }
}
