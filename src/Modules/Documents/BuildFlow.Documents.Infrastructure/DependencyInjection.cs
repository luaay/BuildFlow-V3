using BuildFlow.Documents.Application.Abstractions;
using BuildFlow.Documents.Domain.Repositories;
using BuildFlow.Documents.Infrastructure.Persistence;
using BuildFlow.Documents.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildFlow.Documents.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDocumentsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // السياق بسلسلة اتصال المستندات الخاصّة
        services.AddDbContext<DocumentsDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DocumentsDb")));

        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}