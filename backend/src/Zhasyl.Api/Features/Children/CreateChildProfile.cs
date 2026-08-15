using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Auth;
using Zhasyl.Api.Common.Errors;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;
using Zhasyl.Api.Domain.Identity;

namespace Zhasyl.Api.Features.Children;

public static class CreateChildProfile
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/adult/children", async (
                [FromBody] Body body,
                HttpContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(
                    new Request(context.User.GetActorId(), body.DisplayName, body.LearningLocale),
                    cancellationToken);
                return Results.Created($"/api/adult/children/{response.ChildId}", response);
            }).RequireAuthorization(ActorAuthentication.AdultPolicy);
        }

        private sealed record Body(string DisplayName, string LearningLocale);
    }

    public sealed record Request(
        Guid AdultId,
        string DisplayName,
        string LearningLocale) : IRequest<Response>;

    public sealed record Response(Guid ChildId, string DisplayName, string LearningLocale);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(request => request.DisplayName)
                .NotEmpty().WithErrorCode("children:profile:display_name:required")
                .MaximumLength(60).WithErrorCode("children:profile:display_name:too_long");
            RuleFor(request => request.LearningLocale)
                .Must(locale => locale is "ru" or "kk")
                .WithErrorCode("children:profile:learning_locale:invalid");
        }
    }

    public sealed class RequestHandler(AppDbContext db, TimeProvider timeProvider)
        : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            if (!await db.AdultAccounts.AnyAsync(item => item.Id == request.AdultId, cancellationToken))
            {
                throw new DomainException(404, "Adult account not found", "The adult account does not exist.", "identity:adult:read:not_found");
            }

            var displayName = request.DisplayName.Trim();
            if (await db.ChildProfiles.AnyAsync(item =>
                item.AdultAccountId == request.AdultId && item.DisplayName == displayName,
                cancellationToken))
            {
                throw new DomainException(409, "Child profile already exists", "A child profile with this name already exists.", "children:profile:create:duplicate_name");
            }

            var now = timeProvider.GetUtcNow();
            var profile = new ChildProfile
            {
                Id = Guid.CreateVersion7(),
                AdultAccountId = request.AdultId,
                DisplayName = displayName,
                LearningLocale = request.LearningLocale,
                CreatedAt = now,
                UpdatedAt = now,
            };
            db.ChildProfiles.Add(profile);
            await db.SaveChangesAsync(cancellationToken);
            return new Response(profile.Id, profile.DisplayName, profile.LearningLocale);
        }
    }
}
