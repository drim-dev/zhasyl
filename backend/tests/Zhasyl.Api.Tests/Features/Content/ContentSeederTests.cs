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
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"content-seeder-{Guid.NewGuid():N}")
            .Options;
        await using var db = new AppDbContext(options);
        var source = new MutableSeedSource(CreateDocuments("hash-v1", "Первая версия"));
        var seeder = new ContentSeeder(
            db,
            source,
            TimeProvider.System,
            NullLogger<ContentSeeder>.Instance);

        await seeder.SeedAsync(CancellationToken.None);
        await seeder.SeedAsync(CancellationToken.None);

        Assert.Single(await db.MissionRevisions.ToListAsync());

        source.Documents = CreateDocuments("hash-v2", "Вторая версия");
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
                Assert.Equal("Первая версия", revision.BodyMdx);
            },
            revision =>
            {
                Assert.Equal(2, revision.Version);
                Assert.True(revision.IsCurrent);
                Assert.Equal("Вторая версия", revision.BodyMdx);
            });
    }

    private static IReadOnlyList<ContentSeedDocument> CreateDocuments(
        string missionHash,
        string missionBody) =>
    [
        new(
            ContentKind.Station,
            "zhasyl-1",
            "ru",
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
            true,
            string.Empty,
            "station-hash",
            "station/overview.ru.mdx"),
        new(
            ContentKind.Laboratory,
            "bioinformatics",
            "ru",
            "zhasyl-1",
            null,
            1,
            "Лаборатория биоинформатики",
            null,
            null,
            "Анализирует живые системы",
            "Лариса Ким",
            null,
            null,
            true,
            string.Empty,
            "laboratory-hash",
            "laboratories/bioinformatics/overview.ru.mdx"),
        new(
            ContentKind.Mission,
            "bioscout",
            "ru",
            null,
            "bioinformatics",
            1,
            "BioScout",
            null,
            null,
            null,
            null,
            "Болезнь растений",
            "Подготовка",
            true,
            missionBody,
            missionHash,
            "laboratories/bioinformatics/missions/01-bioscout.ru.mdx"),
    ];

    private sealed class MutableSeedSource(IReadOnlyList<ContentSeedDocument> documents)
        : IContentSeedSource
    {
        public IReadOnlyList<ContentSeedDocument> Documents { get; set; } = documents;

        public Task<IReadOnlyList<ContentSeedDocument>> LoadAsync(
            CancellationToken cancellationToken) => Task.FromResult(Documents);
    }
}
