namespace Zhasyl.Api.Domain.Identity;

public sealed class AdultAccount
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string PreferredLocale { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public ICollection<OAuthIdentity> OAuthIdentities { get; set; } = [];
    public ICollection<ChildProfile> ChildProfiles { get; set; } = [];
}

public sealed class OAuthIdentity
{
    public Guid Id { get; set; }
    public Guid AdultAccountId { get; set; }
    public required string Provider { get; set; }
    public required string ProviderSubject { get; set; }
    public required string ProviderEmail { get; set; }
    public DateTimeOffset LinkedAt { get; set; }
    public AdultAccount AdultAccount { get; set; } = null!;
}
