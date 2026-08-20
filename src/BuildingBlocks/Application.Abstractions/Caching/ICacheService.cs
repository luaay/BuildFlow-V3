namespace BuildFlow.Application.Abstractions.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    // حذف كل المفاتيح التي تبدأ ببادئة — هذا سبب وجود العقد أصلاً
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}