using Loans.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Loans.Infrastructure.Caching;

public sealed class MemoryCacheService(IMemoryCache cache) : ICacheService
{
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);

    public T? Get<T>(string key) =>
        cache.TryGetValue(key, out T? value) ? value : default;

    public void Set<T>(string key, T value, TimeSpan? ttl = null) =>
        cache.Set(key, value, ttl ?? DefaultTtl);

    public void Remove(string key) =>
        cache.Remove(key);
}
