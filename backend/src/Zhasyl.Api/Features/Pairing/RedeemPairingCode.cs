using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Errors;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Database;
using Zhasyl.Api.Domain.Identity;

namespace Zhasyl.Api.Features.Pairing;

public static class RedeemPairingCode
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/child/pair", async (
                [FromBody] Body body,
                ISender sender,
                CancellationToken cancellationToken) =>
                Results.Ok(await sender.Send(
                    new Request(body.Code, body.DeviceName),
                    cancellationToken)))
                .AllowAnonymous()
                .RequireRateLimiting("pairing");
        }

        private sealed record Body(string Code, string DeviceName);
    }

    public sealed record Request(string Code, string DeviceName) : IRequest<Response>;
    public sealed record Response(
        string SessionToken,
        DateTimeOffset ExpiresAt,
        Guid ChildId,
        string DisplayName,
        string LearningLocale);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(request => request.Code)
                .NotEmpty().WithErrorCode("pairing:code:value:required")
                .Must(code => DeviceCredentials.NormalizePairingCode(code).Length == 8)
                .WithErrorCode("pairing:code:value:invalid");
            RuleFor(request => request.DeviceName)
                .NotEmpty().WithErrorCode("pairing:device:name:required")
                .MaximumLength(80).WithErrorCode("pairing:device:name:too_long");
        }
    }

    public sealed class RequestHandler(AppDbContext db, TimeProvider timeProvider)
        : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var normalizedCode = DeviceCredentials.NormalizePairingCode(request.Code);
            var codeHash = DeviceCredentials.Hash(normalizedCode);
            var now = timeProvider.GetUtcNow();
            var pairingCode = await db.DevicePairingCodes
                .Include(item => item.ChildProfile)
                .SingleOrDefaultAsync(item =>
                    item.CodeHash == codeHash &&
                    item.UsedAt == null &&
                    item.ExpiresAt > now,
                    cancellationToken);
            if (pairingCode is null)
            {
                throw new DomainException(400, "Pairing code is invalid", "The pairing code is invalid, expired, or already used.", "pairing:code:redeem:invalid_or_expired");
            }

            pairingCode.UsedAt = now;
            var token = DeviceCredentials.CreateSessionToken();
            var expiresAt = now.AddDays(90);
            db.ChildDeviceSessions.Add(new ChildDeviceSession
            {
                Id = Guid.CreateVersion7(),
                ChildProfileId = pairingCode.ChildProfileId,
                TokenHash = DeviceCredentials.Hash(token),
                DeviceName = request.DeviceName.Trim(),
                CreatedAt = now,
                ExpiresAt = expiresAt,
            });
            await db.SaveChangesAsync(cancellationToken);
            return new Response(
                token,
                expiresAt,
                pairingCode.ChildProfileId,
                pairingCode.ChildProfile.DisplayName,
                pairingCode.ChildProfile.LearningLocale);
        }
    }
}
