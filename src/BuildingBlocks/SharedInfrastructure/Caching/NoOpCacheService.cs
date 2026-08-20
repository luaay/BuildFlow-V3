using BuildFlow.Application.Abstractions.Caching;

namespace BuildFlow.SharedInfrastructure.Caching;

// يُستعمل حين لا يوجد مخزن مؤقّت مضبوط — التطبيق يعمل كأن التخزين معطّل
public sealed class NoOpCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        => Task.FromResult<T?>(default);

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}