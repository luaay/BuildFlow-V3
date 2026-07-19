using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using BuildFlow.Application.Abstractions.Behaviors;

namespace BuildFlow.Projects.Application;

// نقطة تسجيل خدمات طبقة تطبيق المشاريع
public static class DependencyInjection
{
    public static IServiceCollection AddProjectsApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // تسجيل كل معالجات الوسيط في هذه التجميعة تلقائياً
         services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);

            // سجّل سلوك التحقّق في الأنبوب Pipeline
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // تسجيل كل المتحقّقات في هذه التجميعة تلقائياً
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}