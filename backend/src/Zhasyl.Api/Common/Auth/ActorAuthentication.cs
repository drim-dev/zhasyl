using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Zhasyl.Api.Database;
using Zhasyl.Api.Features.Pairing;

namespace Zhasyl.Api.Common.Auth;

public static class ActorAuthentication
{
    public const string AdultScheme = "AdultBff";
    public const string ChildScheme = "ChildDevice";
    public const string AdultPolicy = "Adult";
    public const string ChildPolicy = "Child";
    public const string ActorTypeClaim = "zhasyl_actor";

    public static Guid GetActorId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException("The authenticated actor has no valid identifier.");
    }
}

public sealed class AdultHeaderAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Adult-Id", out var header))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!Guid.TryParse(header, out var adultId))
        {
            return Task.FromResult(AuthenticateResult.Fail("X-Adult-Id is invalid."));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, adultId.ToString()),
            new(ActorAuthentication.ActorTypeClaim, "adult"),
        };
        if (Request.Headers.TryGetValue("X-Adult-Email", out var email))
        {
            claims.Add(new Claim(ClaimTypes.Email, email.ToString()));
        }

        return Task.FromResult(CreateSuccess(claims));
    }

    private AuthenticateResult CreateSuccess(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        return AuthenticateResult.Success(
            new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name));
    }
}

public sealed class ChildDeviceAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    AppDbContext db,
    TimeProvider timeProvider)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Child-Session", out var header))
        {
            return AuthenticateResult.NoResult();
        }

        var tokenHash = DeviceCredentials.Hash(header.ToString());
        var now = timeProvider.GetUtcNow();
        var session = await db.ChildDeviceSessions
            .AsNoTracking()
            .Where(item =>
                item.TokenHash == tokenHash &&
                item.RevokedAt == null &&
                item.ExpiresAt > now)
            .Select(item => new { item.Id, item.ChildProfileId })
            .SingleOrDefaultAsync(Context.RequestAborted);

        if (session is null)
        {
            return AuthenticateResult.Fail("The child device session is invalid or expired.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, session.ChildProfileId.ToString()),
            new Claim(ActorAuthentication.ActorTypeClaim, "child"),
            new Claim("zhasyl_device_session", session.Id.ToString()),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        return AuthenticateResult.Success(
            new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name));
    }
}
