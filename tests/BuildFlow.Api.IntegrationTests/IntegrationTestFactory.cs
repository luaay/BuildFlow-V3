using BuildFlow.Identity.Infrastructure.Persistence;
using BuildFlow.Projects.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BuildFlow.Api.IntegrationTests;

// يشغّل حاوية قاعدة بيانات، ويوجّه التطبيق إليها، وينشئ المخطّط
public sealed class IntegrationTestFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    // حاوية قاعدة بيانات حقيقية للاختبار
   private readonly MsSqlContainer _dbContainer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();
    // توجيه التطبيق إلى قاعدة الحاوية بدل المحلّية
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // أزِل سياقَي الوحدتين المسجّلين على القاعدة المحلّية
            RemoveDbContext<IdentityDbContext>(services);
            RemoveDbContext<ProjectsDbContext>(services);

            var connectionString = _dbContainer.GetConnectionString();

            // أعِد تسجيلهما على قاعدة الحاوية
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDbContext<ProjectsDbContext>(options =>
                options.UseSqlServer(connectionString));
        });
    }

    private static void RemoveDbContext<TContext>(IServiceCollection services)
        where TContext : DbContext
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<TContext>));
        if (descriptor is not null)
            services.Remove(descriptor);
    }

    // يُستدعى قبل الاختبارات: يشغّل الحاوية وينشئ المخطّط
   // يُستدعى قبل الاختبارات: يشغّل الحاوية وينشئ المخطّط
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();

        var identityDb = scope.ServiceProvider
            .GetRequiredService<IdentityDbContext>();
        var projectsDb = scope.ServiceProvider
            .GetRequiredService<ProjectsDbContext>();

        // سياق الهوية ينشئ القاعدة بجداوله
        await identityDb.Database.EnsureCreatedAsync();

        // سياق المشاريع على القاعدة نفسها، فننشئ جداوله مباشرةً
        var projectsCreator = projectsDb.GetService<IRelationalDatabaseCreator>();
        await projectsCreator.CreateTablesAsync();
    }

    // يُستدعى بعد الاختبارات: يزيل الحاوية
    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}