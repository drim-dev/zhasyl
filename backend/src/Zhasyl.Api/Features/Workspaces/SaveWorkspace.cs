using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Auth;
using Zhasyl.Api.Common.Errors;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;
using Zhasyl.Api.Domain.Learning;

namespace Zhasyl.Api.Features.Workspaces;

public static class SaveWorkspace
{
    private const int MaximumBytes = 200_000;

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/child/workspaces/{assignmentRevisionId:guid}", async (
                Guid assignmentRevisionId,
                [FromBody] Body body,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
                Results.Ok(await sender.Send(
                    new Request(
                        context.User.GetActorId(),
                        assignmentRevisionId,
                        body.ExpectedVersion,
                        body.Code),
                    cancellationToken)))
                .RequireAuthorization(ActorAuthentication.ChildPolicy);
        }

        private sealed record Body(int ExpectedVersion, string Code);
    }

    public sealed record Request(
        Guid ChildId,
        Guid AssignmentRevisionId,
        int ExpectedVersion,
        string Code) : IRequest<Response>;
    public sealed record Response(Guid AssignmentRevisionId, int Version, DateTimeOffset SavedAt);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(request => request.ExpectedVersion)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode("workspace:version:value:invalid");
            RuleFor(request => request.Code)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .Must(code => Encoding.UTF8.GetByteCount(code) <= MaximumBytes)
                .WithErrorCode("workspace:code:value:too_large");
        }
    }

    public sealed class RequestHandler(
        AppDbContext db,
        IWorkspaceSnapshotStore snapshots,
        TimeProvider timeProvider) : IRequestHandler<Request, Response>
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
                .SingleOrDefaultAsync(item =>
                    item.ChildProfileId == request.ChildId &&
                    item.StationAssignmentId == assignmentId,
                    cancellationToken);
            if (workspace is null && request.ExpectedVersion != 0 ||
                workspace is not null && workspace.CurrentVersion != request.ExpectedVersion)
            {
                throw Conflict();
            }

            var now = timeProvider.GetUtcNow();
            workspace ??= new LearnerWorkspace
            {
                Id = Guid.CreateVersion7(),
                ChildProfileId = request.ChildId,
                StationAssignmentId = assignmentId.Value,
                AssignmentRevisionId = request.AssignmentRevisionId,
                CurrentVersion = 0,
                CreatedAt = now,
                UpdatedAt = now,
            };
            if (workspace.CurrentVersion == 0)
            {
                db.LearnerWorkspaces.Add(workspace);
            }

            var nextVersion = request.ExpectedVersion + 1;
            var snapshotId = Guid.CreateVersion7();
            var blobName = $"children/{request.ChildId:N}/workspaces/{workspace.Id:N}/versions/{nextVersion:D8}-{snapshotId:N}.py";
            await snapshots.WriteAsync(blobName, request.Code, cancellationToken);

            workspace.CurrentVersion = nextVersion;
            workspace.UpdatedAt = now;
            db.WorkspaceSnapshots.Add(new WorkspaceSnapshot
            {
                Id = snapshotId,
                LearnerWorkspaceId = workspace.Id,
                Version = nextVersion,
                BlobName = blobName,
                ContentHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Code))).ToLowerInvariant(),
                ByteLength = Encoding.UTF8.GetByteCount(request.Code),
                CreatedAt = now,
            });

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw Conflict();
            }
            catch (DbUpdateException) when (request.ExpectedVersion == 0)
            {
                throw new DomainException(409, "Workspace changed", "Another device created this workspace first.", "workspace:save:conflict");
            }

            return new Response(workspace.AssignmentRevisionId, nextVersion, now);
        }

        private static DomainException Conflict() =>
            new(409, "Workspace changed", "The workspace has a newer saved version.", "workspace:save:conflict");
    }
}
