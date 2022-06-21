using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services
{
    [ExcludeFromCodeCoverage]
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Set(string key, string value) =>
            _httpContextAccessor.HttpContext.Session.SetString(key, value);
        public void Set<T>(T model) =>
            Set(typeof(T).Name, JsonSerializer.Serialize(model));

        public string Get(string key) =>
            _httpContextAccessor.HttpContext.Session.GetString(key);

        public T Get<T>()
        {
            var json = Get(typeof(T).Name);
            if (string.IsNullOrEmpty(json)) return default;
            return JsonSerializer.Deserialize<T>(json);
        }

        public void Delete(string key) =>
            _httpContextAccessor.HttpContext.Session.Remove(key);
    }
}
