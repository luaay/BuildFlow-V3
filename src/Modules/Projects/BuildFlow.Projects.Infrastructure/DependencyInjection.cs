using BuildFlow.Projects.Application.Abstractions;
using BuildFlow.Projects.Domain.Repositories;
using BuildFlow.Projects.Infrastructure.Persistence;
using BuildFlow.Projects.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildFlow.SharedInfrastructure.Auditing;


namespace BuildFlow.Projects.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // السياق بسلسلة اتصال المشاريع، مع اعتراض التدقيق
        services.AddDbContext<ProjectsDbContext>((serviceProvider, options) =>
            options
                .UseSqlServer(configuration.GetConnectionString("ProjectsDb"))
                .AddInterceptors(
                    serviceProvider.GetRequiredService<AuditInterceptor>()));

        // 2. المستودع
        services.AddScoped<IProjectRepository, ProjectRepository>();

        // 3. وحدة العمل
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}