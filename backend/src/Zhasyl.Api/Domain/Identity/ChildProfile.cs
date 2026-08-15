using Zhasyl.Api.Domain.Learning;

namespace Zhasyl.Api.Domain.Identity;

public sealed class ChildProfile
{
    public Guid Id { get; set; }
    public Guid AdultAccountId { get; set; }
    public required string DisplayName { get; set; }
    public required string LearningLocale { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public AdultAccount AdultAccount { get; set; } = null!;
    public ICollection<DevicePairingCode> PairingCodes { get; set; } = [];
    public ICollection<ChildDeviceSession> DeviceSessions { get; set; } = [];
    public ICollection<LearnerWorkspace> Workspaces { get; set; } = [];
}

public sealed class DevicePairingCode
{
    public Guid Id { get; set; }
    public Guid ChildProfileId { get; set; }
    public required string CodeHash { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
    public ChildProfile ChildProfile { get; set; } = null!;
}

public sealed class ChildDeviceSession
{
    public Guid Id { get; set; }
    public Guid ChildProfileId { get; set; }
    public required string TokenHash { get; set; }
    public required string DeviceName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public ChildProfile ChildProfile { get; set; } = null!;
}
