using BuildFlow.Application.Abstractions.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Serilog;

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
             Log.Warning(
                "No Redis connection string configured — caching is disabled");

            services.AddSingleton<ICacheService, NoOpCacheService>();
            return services;
        }

        var options = ConfigurationOptions.Parse(connectionString);

        // لا يمنع إقلاع التطبيق إن كان المخزن متوقّفاً
        options.AbortOnConnectFail = false;

        options.ConnectTimeout = 2000;
        options.SyncTimeout = 2000;

        services.AddSingleton<IConnectionMultiplexer>(
            _ => ConnectionMultiplexer.Connect(options));

        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }
}