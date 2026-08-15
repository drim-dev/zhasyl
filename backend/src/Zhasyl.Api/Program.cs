using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Auth;
using Zhasyl.Api.Common.Errors;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Common.Validation;
using Zhasyl.Api.Database;
using Zhasyl.Api.Features.Content.Seeding;
using Zhasyl.Api.Features.Workspaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddExceptionHandler<UnexpectedExceptionHandler>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = ActorAuthentication.AdultScheme;
        options.DefaultChallengeScheme = ActorAuthentication.AdultScheme;
    })
    .AddScheme<AuthenticationSchemeOptions, AdultHeaderAuthenticationHandler>(
        ActorAuthentication.AdultScheme,
        _ => { })
    .AddScheme<AuthenticationSchemeOptions, ChildDeviceAuthenticationHandler>(
        ActorAuthentication.ChildScheme,
        _ => { });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(ActorAuthentication.AdultPolicy, policy =>
    {
        policy.AddAuthenticationSchemes(ActorAuthentication.AdultScheme);
        policy.RequireClaim(ActorAuthentication.ActorTypeClaim, "adult");
    })
    .AddPolicy(ActorAuthentication.ChildPolicy, policy =>
    {
        policy.AddAuthenticationSchemes(ActorAuthentication.ChildScheme);
        policy.RequireClaim(ActorAuthentication.ActorTypeClaim, "child");
    });
builder.Services.AddRateLimiter(options =>
    options.AddPolicy("pairing", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Request.Headers["X-Zhasyl-Client-Address"].FirstOrDefault() ??
            context.Connection.RemoteIpAddress?.ToString() ??
            "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            })));
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("zhasyl")));
builder.AddAzureBlobServiceClient("blobs");
builder.Services.AddSingleton<IWorkspaceSnapshotStore, AzureBlobWorkspaceSnapshotStore>();
builder.Services.Configure<ContentOptions>(builder.Configuration.GetSection("Content"));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IContentSeedSource, MarkdownContentSeedSource>();
builder.Services.AddScoped<ContentSeeder>();

var app = builder.Build();

if (app.Configuration.GetValue("Database:InitializeOnStartup", true))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<ContentSeeder>().SeedAsync(CancellationToken.None);
}

app.UseExceptionHandler();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();
app.MapDefaultEndpoints();

app.Run();

public partial class Program;
