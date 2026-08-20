using System.Text.Json;
using BuildFlow.Application.Abstractions.Caching;
using StackExchange.Redis;

namespace BuildFlow.SharedInfrastructure.Caching;

public sealed class RedisCacheService(IConnectionMultiplexer connection) : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    private readonly IDatabase _database = connection.GetDatabase();

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(key);

        return value.IsNullOrEmpty
            ? default
            : JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(value);

        await _database.StringSetAsync(
            key,
            payload,
            expiration ?? DefaultExpiration);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => _database.KeyDeleteAsync(key);

    public async Task RemoveByPrefixAsync(
        string prefix,
        CancellationToken cancellationToken = default)
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
}