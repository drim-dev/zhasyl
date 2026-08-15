using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Domain.Content;
using Zhasyl.Api.Domain.Identity;
using Zhasyl.Api.Domain.Learning;

namespace Zhasyl.Api.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<StationTranslation> StationTranslations => Set<StationTranslation>();
    public DbSet<Laboratory> Laboratories => Set<Laboratory>();
    public DbSet<LaboratoryTranslation> LaboratoryTranslations => Set<LaboratoryTranslation>();
    public DbSet<Mission> Missions => Set<Mission>();
    public DbSet<MissionRevision> MissionRevisions => Set<MissionRevision>();
    public DbSet<StationAssignment> StationAssignments => Set<StationAssignment>();
    public DbSet<StationAssignmentRevision> StationAssignmentRevisions => Set<StationAssignmentRevision>();
    public DbSet<AdultAccount> AdultAccounts => Set<AdultAccount>();
    public DbSet<OAuthIdentity> OAuthIdentities => Set<OAuthIdentity>();
    public DbSet<ChildProfile> ChildProfiles => Set<ChildProfile>();
    public DbSet<DevicePairingCode> DevicePairingCodes => Set<DevicePairingCode>();
    public DbSet<ChildDeviceSession> ChildDeviceSessions => Set<ChildDeviceSession>();
    public DbSet<LearnerWorkspace> LearnerWorkspaces => Set<LearnerWorkspace>();
    public DbSet<WorkspaceSnapshot> WorkspaceSnapshots => Set<WorkspaceSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
