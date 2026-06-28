using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Identity.Domain.Tenants;
using BuildFlow.Identity.Domain.Users;
using BuildFlow.Identity.Infrastructure.Authentication;
using BuildFlow.Identity.Infrastructure.Persistence;
using BuildFlow.Identity.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildFlow.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. DbContext
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("IdentityDb")));

        // 2. المستودعات
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // 3. Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 4. خدمات المصادقة
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        // 5. إعدادات الـ JWT (من appsettings)
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        return services;
    }
}