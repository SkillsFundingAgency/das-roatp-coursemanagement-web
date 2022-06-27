namespace SFA.DAS.Roatp.CourseManagement.Web.Services
{
    public interface ISessionService
    {
        void Delete(string key);
        string Get(string key);
        T Get<T>();
        void Set(string key, string value);
        void Set<T>(T model);
    }
}