using Microsoft.Extensions.DependencyInjection;

namespace BuildFlow.SharedInfrastructure.Auditing;

// امتدادات التسجيل Registration extensions للتدقيق
public static class AuditExtensions
{
    // نسجّل الاعتراض Interceptor بنطاق scoped
    // تنفيذ المستودع IAuditRepository تسجّله البنية التحتية للوحدات
    public static IServiceCollection AddAuditLogging(this IServiceCollection services)
    {
        services.AddScoped<AuditInterceptor>();
        return services;
    }
}