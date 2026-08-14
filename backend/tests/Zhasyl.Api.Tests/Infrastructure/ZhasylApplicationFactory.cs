using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Zhasyl.Api.Database;
using Zhasyl.Api.Features.Content.Seeding;

namespace Zhasyl.Api.Tests.Infrastructure;

public sealed class ZhasylApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string databaseName = $"zhasyl-tests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:InitializeOnStartup"] = "false",
                ["Content:Root"] = Path.Combine(AppContext.BaseDirectory, "Content"),
            });
        });
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
        scope.ServiceProvider.GetRequiredService<ContentSeeder>()
            .SeedAsync(CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        return host;
    }
}
