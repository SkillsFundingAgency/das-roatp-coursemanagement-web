namespace SFA.DAS.Roatp.CourseManagement.Web.Services
{
    public interface ISessionService
    {
        void Set(string value, string key, string context);
        void Set<T>(T model, string context);
        string Get(string key, string context);
        T Get<T>(string context);
        void Delete(string key, string context);
        void Delete<T>(T model, string context);
        void Clear();
    }
}