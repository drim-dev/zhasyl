using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zhasyl.Api.Domain.Identity;

namespace Zhasyl.Api.Database.Configurations;

public sealed class AdultAccountConfiguration : IEntityTypeConfiguration<AdultAccount>
{
    public void Configure(EntityTypeBuilder<AdultAccount> builder)
    {
        builder.ToTable("adult_accounts");
        builder.HasKey(account => account.Id);
        builder.Property(account => account.Email).HasMaxLength(320).IsRequired();
        builder.Property(account => account.PreferredLocale).HasMaxLength(16).IsRequired();
        builder.HasIndex(account => account.Email).IsUnique();
    }
}

public sealed class OAuthIdentityConfiguration : IEntityTypeConfiguration<OAuthIdentity>
{
    public void Configure(EntityTypeBuilder<OAuthIdentity> builder)
    {
        builder.ToTable("oauth_identities");
        builder.HasKey(identity => identity.Id);
        builder.Property(identity => identity.Provider).HasMaxLength(40).IsRequired();
        builder.Property(identity => identity.ProviderSubject).HasMaxLength(255).IsRequired();
        builder.Property(identity => identity.ProviderEmail).HasMaxLength(320).IsRequired();
        builder.HasIndex(identity => new { identity.Provider, identity.ProviderSubject }).IsUnique();
        builder.HasOne(identity => identity.AdultAccount)
            .WithMany(account => account.OAuthIdentities)
            .HasForeignKey(identity => identity.AdultAccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class ChildProfileConfiguration : IEntityTypeConfiguration<ChildProfile>
{
    public void Configure(EntityTypeBuilder<ChildProfile> builder)
    {
        builder.ToTable("child_profiles");
        builder.HasKey(profile => profile.Id);
        builder.Property(profile => profile.DisplayName).HasMaxLength(60).IsRequired();
        builder.Property(profile => profile.LearningLocale).HasMaxLength(16).IsRequired();
        builder.HasIndex(profile => new { profile.AdultAccountId, profile.DisplayName }).IsUnique();
        builder.HasOne(profile => profile.AdultAccount)
            .WithMany(account => account.ChildProfiles)
            .HasForeignKey(profile => profile.AdultAccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class DevicePairingCodeConfiguration : IEntityTypeConfiguration<DevicePairingCode>
{
    public void Configure(EntityTypeBuilder<DevicePairingCode> builder)
    {
        builder.ToTable("device_pairing_codes");
        builder.HasKey(code => code.Id);
        builder.Property(code => code.CodeHash).HasMaxLength(64).IsRequired();
        builder.HasIndex(code => code.CodeHash).IsUnique();
        builder.HasIndex(code => new { code.ChildProfileId, code.ExpiresAt });
        builder.HasOne(code => code.ChildProfile)
            .WithMany(profile => profile.PairingCodes)
            .HasForeignKey(code => code.ChildProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class ChildDeviceSessionConfiguration : IEntityTypeConfiguration<ChildDeviceSession>
{
    public void Configure(EntityTypeBuilder<ChildDeviceSession> builder)
    {
        builder.ToTable("child_device_sessions");
        builder.HasKey(session => session.Id);
        builder.Property(session => session.TokenHash).HasMaxLength(64).IsRequired();
        builder.Property(session => session.DeviceName).HasMaxLength(80).IsRequired();
        builder.HasIndex(session => session.TokenHash).IsUnique();
        builder.HasIndex(session => new { session.ChildProfileId, session.ExpiresAt });
        builder.HasOne(session => session.ChildProfile)
            .WithMany(profile => profile.DeviceSessions)
            .HasForeignKey(session => session.ChildProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
