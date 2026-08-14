using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zhasyl.Api.Domain.Content;

namespace Zhasyl.Api.Database.Configurations;

public sealed class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.ToTable("stations");
        builder.HasKey(station => station.Id);
        builder.Property(station => station.Slug).HasMaxLength(80).IsRequired();
        builder.HasIndex(station => station.Slug).IsUnique();
    }
}

public sealed class StationTranslationConfiguration : IEntityTypeConfiguration<StationTranslation>
{
    public void Configure(EntityTypeBuilder<StationTranslation> builder)
    {
        builder.ToTable("station_translations");
        builder.HasKey(translation => new { translation.StationId, translation.Locale });
        builder.Property(translation => translation.Locale).HasMaxLength(16).IsRequired();
        builder.Property(translation => translation.Name).HasMaxLength(160).IsRequired();
        builder.Property(translation => translation.Location).HasMaxLength(240).IsRequired();
        builder.Property(translation => translation.Briefing).HasMaxLength(2_000).IsRequired();
        builder.HasOne(translation => translation.Station)
            .WithMany(station => station.Translations)
            .HasForeignKey(translation => translation.StationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class LaboratoryConfiguration : IEntityTypeConfiguration<Laboratory>
{
    public void Configure(EntityTypeBuilder<Laboratory> builder)
    {
        builder.ToTable("laboratories");
        builder.HasKey(laboratory => laboratory.Id);
        builder.Property(laboratory => laboratory.Slug).HasMaxLength(80).IsRequired();
        builder.HasIndex(laboratory => new { laboratory.StationId, laboratory.Slug }).IsUnique();
        builder.HasIndex(laboratory => new { laboratory.StationId, laboratory.Order }).IsUnique();
        builder.HasOne(laboratory => laboratory.Station)
            .WithMany(station => station.Laboratories)
            .HasForeignKey(laboratory => laboratory.StationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class LaboratoryTranslationConfiguration : IEntityTypeConfiguration<LaboratoryTranslation>
{
    public void Configure(EntityTypeBuilder<LaboratoryTranslation> builder)
    {
        builder.ToTable("laboratory_translations");
        builder.HasKey(translation => new { translation.LaboratoryId, translation.Locale });
        builder.Property(translation => translation.Locale).HasMaxLength(16).IsRequired();
        builder.Property(translation => translation.Name).HasMaxLength(160).IsRequired();
        builder.Property(translation => translation.Purpose).HasMaxLength(1_000).IsRequired();
        builder.Property(translation => translation.Specialist).HasMaxLength(160).IsRequired();
        builder.HasOne(translation => translation.Laboratory)
            .WithMany(laboratory => laboratory.Translations)
            .HasForeignKey(translation => translation.LaboratoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class MissionConfiguration : IEntityTypeConfiguration<Mission>
{
    public void Configure(EntityTypeBuilder<Mission> builder)
    {
        builder.ToTable("missions");
        builder.HasKey(mission => mission.Id);
        builder.Property(mission => mission.Slug).HasMaxLength(100).IsRequired();
        builder.HasIndex(mission => new { mission.LaboratoryId, mission.Slug }).IsUnique();
        builder.HasIndex(mission => new { mission.LaboratoryId, mission.Order }).IsUnique();
        builder.HasOne(mission => mission.Laboratory)
            .WithMany(laboratory => laboratory.Missions)
            .HasForeignKey(mission => mission.LaboratoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class MissionRevisionConfiguration : IEntityTypeConfiguration<MissionRevision>
{
    public void Configure(EntityTypeBuilder<MissionRevision> builder)
    {
        builder.ToTable("mission_revisions");
        builder.HasKey(revision => revision.Id);
        builder.Property(revision => revision.Locale).HasMaxLength(16).IsRequired();
        builder.Property(revision => revision.Name).HasMaxLength(200).IsRequired();
        builder.Property(revision => revision.Problem).HasMaxLength(2_000).IsRequired();
        builder.Property(revision => revision.Status).HasMaxLength(120).IsRequired();
        builder.Property(revision => revision.BodyMdx).IsRequired();
        builder.Property(revision => revision.ContentHash).HasMaxLength(64).IsRequired();
        builder.HasIndex(revision => new { revision.MissionId, revision.Locale, revision.Version })
            .IsUnique();
        builder.HasIndex(revision => new { revision.MissionId, revision.Locale })
            .IsUnique()
            .HasFilter("\"IsCurrent\"");
        builder.HasOne(revision => revision.Mission)
            .WithMany(mission => mission.Revisions)
            .HasForeignKey(revision => revision.MissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class StationAssignmentConfiguration : IEntityTypeConfiguration<StationAssignment>
{
    public void Configure(EntityTypeBuilder<StationAssignment> builder)
    {
        builder.ToTable("station_assignments");
        builder.HasKey(assignment => assignment.Id);
        builder.Property(assignment => assignment.Slug).HasMaxLength(100).IsRequired();
        builder.HasIndex(assignment => new { assignment.MissionId, assignment.Slug }).IsUnique();
        builder.HasIndex(assignment => new { assignment.MissionId, assignment.Order }).IsUnique();
        builder.HasOne(assignment => assignment.Mission)
            .WithMany(mission => mission.Assignments)
            .HasForeignKey(assignment => assignment.MissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class StationAssignmentRevisionConfiguration
    : IEntityTypeConfiguration<StationAssignmentRevision>
{
    public void Configure(EntityTypeBuilder<StationAssignmentRevision> builder)
    {
        builder.ToTable("station_assignment_revisions");
        builder.HasKey(revision => revision.Id);
        builder.Property(revision => revision.Locale).HasMaxLength(16).IsRequired();
        builder.Property(revision => revision.Name).HasMaxLength(200).IsRequired();
        builder.Property(revision => revision.Objective).HasMaxLength(2_000).IsRequired();
        builder.Property(revision => revision.BodyMdx).IsRequired();
        builder.Property(revision => revision.ContentHash).HasMaxLength(64).IsRequired();
        builder.HasIndex(revision => new
        {
            revision.StationAssignmentId,
            revision.Locale,
            revision.Version,
        })
            .IsUnique();
        builder.HasIndex(revision => new { revision.StationAssignmentId, revision.Locale })
            .IsUnique()
            .HasFilter("\"IsCurrent\"");
        builder.HasOne(revision => revision.StationAssignment)
            .WithMany(assignment => assignment.Revisions)
            .HasForeignKey(revision => revision.StationAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
