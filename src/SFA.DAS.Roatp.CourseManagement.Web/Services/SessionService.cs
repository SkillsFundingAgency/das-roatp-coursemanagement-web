using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public const string ContextKey = "ContextKey";

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Set(string value, string key, string context)
        {
            _httpContextAccessor.HttpContext.Session.SetString(ContextKey, context);
            _httpContextAccessor.HttpContext.Session.SetString(key, value);
        }
        public void Set<T>(T model, string context) => Set(JsonSerializer.Serialize(model), typeof(T).Name, context);

        public string Get(string key, string context)
        {
            var contextValue = _httpContextAccessor.HttpContext.Session.GetString(ContextKey);
            if (contextValue == context)
                return _httpContextAccessor.HttpContext.Session.GetString(key);
            else
                return default;
        }

        public T Get<T>(string context)
        {
            var json = Get(typeof(T).Name, context);
            if (string.IsNullOrEmpty(json)) return default;
            return JsonSerializer.Deserialize<T>(json);
        }

        public void Delete(string key, string context)
        {
            var contextValue = _httpContextAccessor.HttpContext.Session.GetString(ContextKey);
            if (contextValue == context && _httpContextAccessor.HttpContext.Session.Keys.Any(k => k == key))
                _httpContextAccessor.HttpContext.Session.Remove(key);
        }

        public void Delete<T>(T model, string context) => Delete(typeof(T).Name, context);

        public void Clear() => _httpContextAccessor.HttpContext.Session.Clear();

    }
}
