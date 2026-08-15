using MediatR;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Auth;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;

namespace Zhasyl.Api.Features.Children;

public static class ListChildProfiles
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/adult/children", async (
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
                Results.Ok(await sender.Send(
                    new Request(context.User.GetActorId()),
                    cancellationToken)))
                .RequireAuthorization(ActorAuthentication.AdultPolicy);
        }
    }

    public sealed record Request(Guid AdultId) : IRequest<Response>;
    public sealed record Response(IReadOnlyList<ChildItem> Children);
    public sealed record ChildItem(
        Guid ChildId,
        string DisplayName,
        string LearningLocale,
        IReadOnlyList<DeviceItem> Devices);
    public sealed record DeviceItem(
        Guid DeviceId,
        string DeviceName,
        DateTimeOffset CreatedAt,
        DateTimeOffset ExpiresAt,
        bool IsRevoked);

    public sealed class RequestHandler(AppDbContext db) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var children = await db.ChildProfiles
                .AsNoTracking()
                .Where(profile => profile.AdultAccountId == request.AdultId)
                .OrderBy(profile => profile.CreatedAt)
                .Select(profile => new ChildItem(
                    profile.Id,
                    profile.DisplayName,
                    profile.LearningLocale,
                    profile.DeviceSessions
                        .OrderByDescending(session => session.CreatedAt)
                        .Select(session => new DeviceItem(
                            session.Id,
                            session.DeviceName,
                            session.CreatedAt,
                            session.ExpiresAt,
                            session.RevokedAt != null))
                        .ToList()))
                .ToListAsync(cancellationToken);
            return new Response(children);
        }
    }
}
