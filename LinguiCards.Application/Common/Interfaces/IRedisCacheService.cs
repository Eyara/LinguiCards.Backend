namespace LinguiCards.Application.Common.Interfaces;

public interface IRedisCacheService
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
}