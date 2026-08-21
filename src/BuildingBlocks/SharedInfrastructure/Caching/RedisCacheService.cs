using System.Text.Json;
using BuildFlow.Application.Abstractions.Caching;
using Serilog;
using StackExchange.Redis;

namespace BuildFlow.SharedInfrastructure.Caching;

// المخزن تحسين لا اعتماد: أي إخفاق فيه يعود بالطلب إلى المصدر الأصلي
public sealed class RedisCacheService(IConnectionMultiplexer connection) : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    private readonly IDatabase _database = connection.GetDatabase();

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _database.StringGetAsync(key);

            return value.IsNullOrEmpty
                ? default
                : JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception exception)
        {
            // إخفاق القراءة يُعامَل كإخفاق إصابة — يُقرأ من المصدر
            Log.Warning(exception, "Cache read failed for {CacheKey}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = JsonSerializer.Serialize(value);

            await _database.StringSetAsync(
                key,
                payload,
                expiration ?? DefaultExpiration);
        }
        catch (Exception exception)
        {
            Log.Warning(exception, "Cache write failed for {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception exception)
        {
            Log.Warning(exception, "Cache removal failed for {CacheKey}", key);
        }
    }

    public async Task RemoveByPrefixAsync(
        string prefix,
        CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var endpoint in connection.GetEndPoints())
            {
                var server = connection.GetServer(endpoint);

                await foreach (var key in server.KeysAsync(pattern: $"{prefix}*")
                                   .WithCancellation(cancellationToken))
                {
                    await _database.KeyDeleteAsync(key);
                }
            }
        }
        catch (Exception exception)
        {
            Log.Warning(exception, "Cache invalidation failed for {CachePrefix}", prefix);
        }
    }
}