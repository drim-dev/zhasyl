using MediatR;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Auth;
using Zhasyl.Api.Common.Errors;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;

namespace Zhasyl.Api.Features.Pairing;

public static class RevokeChildDevice
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/adult/children/{childId:guid}/devices/{deviceId:guid}", async (
                Guid childId,
                Guid deviceId,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                await sender.Send(
                    new Request(context.User.GetActorId(), childId, deviceId),
                    cancellationToken);
                return Results.NoContent();
            }).RequireAuthorization(ActorAuthentication.AdultPolicy);
        }
    }

    public sealed record Request(Guid AdultId, Guid ChildId, Guid DeviceId) : IRequest;

    public sealed class RequestHandler(AppDbContext db, TimeProvider timeProvider)
        : IRequestHandler<Request>
    {
        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            var session = await db.ChildDeviceSessions.SingleOrDefaultAsync(item =>
                item.Id == request.DeviceId &&
                item.ChildProfileId == request.ChildId &&
                item.ChildProfile.AdultAccountId == request.AdultId,
                cancellationToken);
            if (session is null)
            {
                throw new DomainException(404, "Device not found", "The paired device does not exist.", "pairing:device:revoke:not_found");
            }

            session.RevokedAt ??= timeProvider.GetUtcNow();
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
