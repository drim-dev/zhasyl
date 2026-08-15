using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zhasyl.Api.Domain.Learning;

namespace Zhasyl.Api.Database.Configurations;

public sealed class LearnerWorkspaceConfiguration : IEntityTypeConfiguration<LearnerWorkspace>
{
    public void Configure(EntityTypeBuilder<LearnerWorkspace> builder)
    {
        builder.ToTable("learner_workspaces");
        builder.HasKey(workspace => workspace.Id);
        builder.Property(workspace => workspace.CurrentVersion).IsConcurrencyToken();
        builder.HasIndex(workspace => new { workspace.ChildProfileId, workspace.StationAssignmentId }).IsUnique();
        builder.HasOne(workspace => workspace.ChildProfile)
            .WithMany(profile => profile.Workspaces)
            .HasForeignKey(workspace => workspace.ChildProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(workspace => workspace.StationAssignment)
            .WithMany()
            .HasForeignKey(workspace => workspace.StationAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(workspace => workspace.AssignmentRevision)
            .WithMany()
            .HasForeignKey(workspace => workspace.AssignmentRevisionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class WorkspaceSnapshotConfiguration : IEntityTypeConfiguration<WorkspaceSnapshot>
{
    public void Configure(EntityTypeBuilder<WorkspaceSnapshot> builder)
    {
        builder.ToTable("workspace_snapshots");
        builder.HasKey(snapshot => snapshot.Id);
        builder.Property(snapshot => snapshot.BlobName).HasMaxLength(512).IsRequired();
        builder.Property(snapshot => snapshot.ContentHash).HasMaxLength(64).IsRequired();
        builder.HasIndex(snapshot => new { snapshot.LearnerWorkspaceId, snapshot.Version }).IsUnique();
        builder.HasOne(snapshot => snapshot.LearnerWorkspace)
            .WithMany(workspace => workspace.Snapshots)
            .HasForeignKey(snapshot => snapshot.LearnerWorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
