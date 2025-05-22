using System.Text.Json;
using LinguiCards.Application.Common.Interfaces;
using StackExchange.Redis;

namespace LinguiCards.Infrastructure.Integration.Caching;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        return _db.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _db.StringGetAsync(key);
        return json.HasValue ? JsonSerializer.Deserialize<T>(json!) : default;
    }
}