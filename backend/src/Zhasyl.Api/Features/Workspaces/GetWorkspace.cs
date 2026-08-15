using MediatR;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Auth;
using Zhasyl.Api.Common.Errors;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;

namespace Zhasyl.Api.Features.Workspaces;

public static class GetWorkspace
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/child/workspaces/{assignmentRevisionId:guid}", async (
                Guid assignmentRevisionId,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
                Results.Ok(await sender.Send(
                    new Request(context.User.GetActorId(), assignmentRevisionId),
                    cancellationToken)))
                .RequireAuthorization(ActorAuthentication.ChildPolicy);
        }
    }

    public sealed record Request(Guid ChildId, Guid AssignmentRevisionId) : IRequest<Response>;
    public sealed record Response(Guid AssignmentRevisionId, int Version, string? Code, DateTimeOffset? SavedAt);

    public sealed class RequestHandler(AppDbContext db, IWorkspaceSnapshotStore snapshots)
        : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var assignmentId = await db.StationAssignmentRevisions
                .AsNoTracking()
                .Where(revision => revision.Id == request.AssignmentRevisionId && revision.PublishedAt != null)
                .Select(revision => (Guid?)revision.StationAssignmentId)
                .SingleOrDefaultAsync(cancellationToken);
            if (assignmentId is null)
            {
                throw new DomainException(404, "Assignment revision not found", "The assignment revision is not published.", "workspace:assignment:read:not_found");
            }

            var workspace = await db.LearnerWorkspaces
                .AsNoTracking()
                .Where(item => item.ChildProfileId == request.ChildId && item.StationAssignmentId == assignmentId)
                .Select(item => new
                {
                    item.AssignmentRevisionId,
                    item.CurrentVersion,
                    SavedAt = item.UpdatedAt,
                    BlobName = item.Snapshots
                        .Where(snapshot => snapshot.Version == item.CurrentVersion)
                        .Select(snapshot => snapshot.BlobName)
                        .Single(),
                })
                .SingleOrDefaultAsync(cancellationToken);
            if (workspace is null)
            {
                return new Response(request.AssignmentRevisionId, 0, null, null);
            }

            var code = await snapshots.ReadAsync(workspace.BlobName, cancellationToken);
            return new Response(
                workspace.AssignmentRevisionId,
                workspace.CurrentVersion,
                code,
                workspace.SavedAt);
        }
    }
}
