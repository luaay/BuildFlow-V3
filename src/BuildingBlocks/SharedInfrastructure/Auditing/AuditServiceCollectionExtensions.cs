using BuildFlow.SharedKernel.Domain.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace BuildFlow.SharedInfrastructure.Auditing;

// امتداد تركيب التدقيق Audit composition، يجمع كل تسجيله في مكان واحد
public static class AuditServiceCollectionExtensions
{
    public static IServiceCollection AddAuditing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // سياق التدقيق DbContext، على سلسلة اتصال التدقيق
        services.AddDbContext<AuditDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("AuditDb")));

        // تنفيذ المستودع Repository
        services.AddScoped<IAuditRepository, AuditRepository>();

        // الاعتراض Interceptor، بنطاق الطلب scoped
        services.AddScoped<AuditInterceptor>();

        // نسجّل معالجات MediatR في هذا المشروع، ليجد معالج استعلام التدقيق
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(AuditServiceCollectionExtensions).Assembly));

        return services;
    }
}