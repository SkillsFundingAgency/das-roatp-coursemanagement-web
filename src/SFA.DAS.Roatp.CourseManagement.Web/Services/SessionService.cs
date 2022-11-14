using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text.Json;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Set(string value, string key)
        {
            _httpContextAccessor.HttpContext.Session.SetString(key, value);
        }
        public void Set<T>(T model) => Set(JsonSerializer.Serialize(model), typeof(T).Name);

        public string Get(string key)
        {
            return _httpContextAccessor.HttpContext.Session.GetString(key);
        }

        public T Get<T>()
        {
            var json = Get(typeof(T).Name);
            if (string.IsNullOrEmpty(json)) return default;
            return JsonSerializer.Deserialize<T>(json);
        }

        public void Delete(string key)
        {
            if (_httpContextAccessor.HttpContext.Session.Keys.Any(k => k == key))
                _httpContextAccessor.HttpContext.Session.Remove(key);
        }

        public void Delete<T>(T model) => Delete(typeof(T).Name);

        public void Clear() => _httpContextAccessor.HttpContext.Session.Clear();
    }
}
