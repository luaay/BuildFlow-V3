using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using BuildFlow.Application.Abstractions.Behaviors;

namespace BuildFlow.Identity.Application;

// نقطة تسجيل خدمات طبقة Identity Application
public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // تسجيل كل الـ MediatR handlers في هذه الـ assembly تلقائياً
       services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);

            // سجّل سلوك التحقّق في الأنبوب Pipeline
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(PerformanceBehavior<,>));
           
            
        });

        // تسجيل كل الـ FluentValidation validators في هذه الـ assembly تلقائياً
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}