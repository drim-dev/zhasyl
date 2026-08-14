namespace Zhasyl.Api.Domain.Content;

public sealed class Mission
{
    public Guid Id { get; set; }
    public Guid LaboratoryId { get; set; }
    public required string Slug { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public Laboratory Laboratory { get; set; } = null!;
    public ICollection<MissionRevision> Revisions { get; set; } = [];
    public ICollection<StationAssignment> Assignments { get; set; } = [];
}

public sealed class MissionRevision
{
    public Guid Id { get; set; }
    public Guid MissionId { get; set; }
    public required string Locale { get; set; }
    public int Version { get; set; }
    public required string Name { get; set; }
    public required string Problem { get; set; }
    public required string Status { get; set; }
    public required string BodyMdx { get; set; }
    public required string ContentHash { get; set; }
    public bool IsCurrent { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public Mission Mission { get; set; } = null!;
}
