using System.Reflection;
using BuildFlow.Application.Abstractions.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BuildFlow.Documents.Application;

// نقطة تسجيل خدمات طبقة تطبيق المستندات
public static class DependencyInjection
{
    public static IServiceCollection AddDocumentsApplication(
        this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);

            // الترتيب مهمّ: التسجيل أوّلاً ليرى كل الطلبات
            config.AddOpenBehavior(typeof(TracingBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(PerformanceBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}