using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BuildFlow.Projects.Application;

// نقطة تسجيل خدمات طبقة تطبيق المشاريع
public static class DependencyInjection
{
    public static IServiceCollection AddProjectsApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // تسجيل كل معالجات الوسيط في هذه التجميعة تلقائياً
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(assembly));

        // تسجيل كل المتحقّقات في هذه التجميعة تلقائياً
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}