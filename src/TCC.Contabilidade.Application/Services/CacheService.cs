using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.Application.Services;

public class CacheService
{
    private readonly ICacheService _cacheProvider;

    public CacheService(ICacheService cacheProvider)
    {
        _cacheProvider = cacheProvider;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        return await _cacheProvider.GetAsync<T>(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        await _cacheProvider.SetAsync(key, value, expiration);
    }

    public async Task RemoveAsync(string key)
    {
        await _cacheProvider.RemoveAsync(key);
    }

    public string GenerateKey(string prefix, params object[] args)
    {
        return $"{prefix}:{string.Join(":", args)}";
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var newValue = await factory();
        if (newValue != null)
        {
            await SetAsync(key, newValue, expiration);
        }

        return newValue;
    }
}
