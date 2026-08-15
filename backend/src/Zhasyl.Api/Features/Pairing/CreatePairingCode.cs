using MediatR;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Auth;
using Zhasyl.Api.Common.Errors;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;
using Zhasyl.Api.Domain.Identity;

namespace Zhasyl.Api.Features.Pairing;

public static class CreatePairingCode
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/adult/children/{childId:guid}/pairing-codes", async (
                Guid childId,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
                Results.Ok(await sender.Send(
                    new Request(context.User.GetActorId(), childId),
                    cancellationToken)))
                .RequireAuthorization(ActorAuthentication.AdultPolicy);
        }
    }

    public sealed record Request(Guid AdultId, Guid ChildId) : IRequest<Response>;
    public sealed record Response(string Code, DateTimeOffset ExpiresAt);

    public sealed class RequestHandler(AppDbContext db, TimeProvider timeProvider)
        : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var childExists = await db.ChildProfiles.AnyAsync(profile =>
                profile.Id == request.ChildId && profile.AdultAccountId == request.AdultId,
                cancellationToken);
            if (!childExists)
            {
                throw new DomainException(404, "Child profile not found", "The child profile does not exist.", "children:profile:read:not_found");
            }

            var code = DeviceCredentials.CreatePairingCode();
            var now = timeProvider.GetUtcNow();
            db.DevicePairingCodes.Add(new DevicePairingCode
            {
                Id = Guid.CreateVersion7(),
                ChildProfileId = request.ChildId,
                CodeHash = DeviceCredentials.Hash(DeviceCredentials.NormalizePairingCode(code)),
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(10),
            });
            await db.SaveChangesAsync(cancellationToken);
            return new Response(code, now.AddMinutes(10));
        }
    }
}
