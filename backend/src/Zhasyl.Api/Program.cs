using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Zhasyl.Api.Common.Errors;
using Zhasyl.Api.Common.Http;
using Zhasyl.Api.Common.Validation;
using Zhasyl.Api.Database;
using Zhasyl.Api.Features.Content.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<UnexpectedExceptionHandler>();
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("zhasyl")));
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
app.MapEndpoints();
app.MapDefaultEndpoints();

app.Run();

public partial class Program;
