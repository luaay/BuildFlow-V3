using BuildFlow.Application.Abstractions.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BuildFlow.SharedInfrastructure.Caching;

public static class CachingServiceCollectionExtensions
{
    public static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddSingleton<ICacheService, NoOpCacheService>();
            return services;
        }

        var options = ConfigurationOptions.Parse(connectionString);

        // لا يمنع إقلاع التطبيق إن كان المخزن متوقّفاً
        options.AbortOnConnectFail = false;

        services.AddSingleton<IConnectionMultiplexer>(
            _ => ConnectionMultiplexer.Connect(options));

        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }
}