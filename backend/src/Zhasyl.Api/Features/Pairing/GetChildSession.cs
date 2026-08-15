using MediatR;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Auth;
using Zhasyl.Api.Common.Errors;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;

namespace Zhasyl.Api.Features.Pairing;

public static class GetChildSession
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/child/session", async (
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
                Results.Ok(await sender.Send(
                    new Request(context.User.GetActorId()),
                    cancellationToken)))
                .RequireAuthorization(ActorAuthentication.ChildPolicy);
        }
    }

    public sealed record Request(Guid ChildId) : IRequest<Response>;
    public sealed record Response(Guid ChildId, string DisplayName, string LearningLocale);

    public sealed class RequestHandler(AppDbContext db) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var child = await db.ChildProfiles
                .AsNoTracking()
                .Where(profile => profile.Id == request.ChildId)
                .Select(profile => new Response(profile.Id, profile.DisplayName, profile.LearningLocale))
                .SingleOrDefaultAsync(cancellationToken);
            return child ?? throw new DomainException(404, "Child profile not found", "The child profile does not exist.", "children:profile:read:not_found");
        }
    }
}
