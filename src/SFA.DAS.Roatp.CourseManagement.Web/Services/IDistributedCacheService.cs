using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services;
public interface IDistributedCacheService
{
    Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getData, TimeSpan cacheDuration);
    Task RemoveAsync(string cacheKey);
}
