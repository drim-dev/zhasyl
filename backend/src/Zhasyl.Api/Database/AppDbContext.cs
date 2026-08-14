using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Domain.Content;

namespace Zhasyl.Api.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<StationTranslation> StationTranslations => Set<StationTranslation>();
    public DbSet<Laboratory> Laboratories => Set<Laboratory>();
    public DbSet<LaboratoryTranslation> LaboratoryTranslations => Set<LaboratoryTranslation>();
    public DbSet<Mission> Missions => Set<Mission>();
    public DbSet<MissionRevision> MissionRevisions => Set<MissionRevision>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
