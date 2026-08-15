using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Errors;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;
using Zhasyl.Api.Domain.Identity;

namespace Zhasyl.Api.Features.Identity;

public static class HandleOAuthSignIn
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/auth/oauth-sign-in", async (
                [FromBody] Body body,
                ISender sender,
                CancellationToken cancellationToken) =>
                Results.Ok(await sender.Send(
                    new Request(body.Provider, body.ProviderUserId, body.ProviderEmail),
                    cancellationToken)))
                .AllowAnonymous();
        }

        private sealed record Body(string Provider, string ProviderUserId, string ProviderEmail);
    }

    public sealed record Request(
        string Provider,
        string ProviderUserId,
        string ProviderEmail) : IRequest<Response>;

    public sealed record Response(Guid AdultId, string Email, string PreferredLocale);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(request => request.Provider)
                .NotEmpty().WithErrorCode("identity:oauth:provider:required")
                .Must(provider => provider is "google" or "github" or "gitlab" or "development")
                .WithErrorCode("identity:oauth:provider:invalid");
            RuleFor(request => request.ProviderUserId)
                .NotEmpty().WithErrorCode("identity:oauth:provider_user_id:required")
                .MaximumLength(255).WithErrorCode("identity:oauth:provider_user_id:too_long");
            RuleFor(request => request.ProviderEmail)
                .NotEmpty().WithErrorCode("identity:oauth:email:required")
                .EmailAddress().WithErrorCode("identity:oauth:email:invalid")
                .MaximumLength(320).WithErrorCode("identity:oauth:email:too_long");
        }
    }

    public sealed class RequestHandler(
        AppDbContext db,
        TimeProvider timeProvider,
        IHostEnvironment environment) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var provider = request.Provider.ToLowerInvariant();
            if (provider == "development" && environment.IsProduction())
            {
                throw new UnauthorizedAccessException("Development sign-in is disabled.");
            }

            var email = request.ProviderEmail.Trim().ToLowerInvariant();
            var identity = await db.OAuthIdentities
                .Include(item => item.AdultAccount)
                .SingleOrDefaultAsync(item =>
                    item.Provider == provider && item.ProviderSubject == request.ProviderUserId,
                    cancellationToken);
            if (identity is not null)
            {
                return ToResponse(identity.AdultAccount);
            }

            var now = timeProvider.GetUtcNow();
            var account = await db.AdultAccounts
                .Include(item => item.OAuthIdentities)
                .SingleOrDefaultAsync(item => item.Email == email, cancellationToken);
            if (account?.OAuthIdentities.Count > 0)
            {
                throw new DomainException(
                    409,
                    "Sign-in method does not match",
                    "This email is already associated with another sign-in identity.",
                    "identity:oauth:sign_in:provider_mismatch");
            }
            if (account is null)
            {
                account = new AdultAccount
                {
                    Id = Guid.CreateVersion7(),
                    Email = email,
                    PreferredLocale = "ru",
                    CreatedAt = now,
                    UpdatedAt = now,
                };
                db.AdultAccounts.Add(account);
            }

            db.OAuthIdentities.Add(new OAuthIdentity
            {
                Id = Guid.CreateVersion7(),
                AdultAccount = account,
                Provider = provider,
                ProviderSubject = request.ProviderUserId,
                ProviderEmail = email,
                LinkedAt = now,
            });
            account.UpdatedAt = now;
            await db.SaveChangesAsync(cancellationToken);
            return ToResponse(account);
        }

        private static Response ToResponse(AdultAccount account) =>
            new(account.Id, account.Email, account.PreferredLocale);
    }
}
