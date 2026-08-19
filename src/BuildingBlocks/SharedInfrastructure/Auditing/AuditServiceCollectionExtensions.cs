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
        // سياق التدقيق، على سلسلة التدقيق إن وُجدت، وإلا على سلسلة مشتركة
        // على الاستضافة، AuditDb قد تغيب، فنتراجع إلى IdentityDb الموجودة
        var auditConnection =
            configuration.GetConnectionString("AuditDb")
            ?? configuration.GetConnectionString("IdentityDb");

        services.AddDbContext<AuditDbContext>(options =>
            options.UseSqlServer(auditConnection));

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