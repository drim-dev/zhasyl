using Zhasyl.Api.Domain.Content;
using Zhasyl.Api.Domain.Identity;

namespace Zhasyl.Api.Domain.Learning;

public sealed class LearnerWorkspace
{
    public Guid Id { get; set; }
    public Guid ChildProfileId { get; set; }
    public Guid StationAssignmentId { get; set; }
    public Guid AssignmentRevisionId { get; set; }
    public int CurrentVersion { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public ChildProfile ChildProfile { get; set; } = null!;
    public StationAssignment StationAssignment { get; set; } = null!;
    public StationAssignmentRevision AssignmentRevision { get; set; } = null!;
    public ICollection<WorkspaceSnapshot> Snapshots { get; set; } = [];
}

public sealed class WorkspaceSnapshot
{
    public Guid Id { get; set; }
    public Guid LearnerWorkspaceId { get; set; }
    public int Version { get; set; }
    public required string BlobName { get; set; }
    public required string ContentHash { get; set; }
    public int ByteLength { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public LearnerWorkspace LearnerWorkspace { get; set; } = null!;
}
