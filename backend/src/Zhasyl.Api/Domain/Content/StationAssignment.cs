namespace Zhasyl.Api.Domain.Content;

public sealed class StationAssignment
{
    public Guid Id { get; set; }
    public Guid MissionId { get; set; }
    public required string Slug { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public Mission Mission { get; set; } = null!;
    public ICollection<StationAssignmentRevision> Revisions { get; set; } = [];
}

public sealed class StationAssignmentRevision
{
    public Guid Id { get; set; }
    public Guid StationAssignmentId { get; set; }
    public required string Locale { get; set; }
    public int Version { get; set; }
    public required string Name { get; set; }
    public required string Objective { get; set; }
    public int EstimatedMinutes { get; set; }
    public required string BodyMdx { get; set; }
    public required string ContentHash { get; set; }
    public bool IsCurrent { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public StationAssignment StationAssignment { get; set; } = null!;
}
