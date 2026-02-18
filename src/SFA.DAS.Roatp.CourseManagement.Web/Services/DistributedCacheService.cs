using Microsoft.Extensions.Caching.Distributed;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services;

public class DistributedCacheService : IDistributedCacheService
{
    private readonly IDistributedCache _distributedCache;

    public DistributedCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getData, TimeSpan cacheDuration)
    {
        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<T>(cachedData);
        }

        var data = await getData();
        var serializedData = JsonSerializer.Serialize(data);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheDuration
        };

        await _distributedCache.SetStringAsync(cacheKey, serializedData, options);

        return data;
    }

    public async Task RemoveAsync(string cacheKey)
    {
        await _distributedCache.RemoveAsync(cacheKey);
    }
}
