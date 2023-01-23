using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Domain.Interfaces
{
    public interface IApiClient
    {
        Task<T> Get<T>(string uri);
        Task<HttpStatusCode> Get(string uri);
        Task<HttpStatusCode> Post<T>(string uri, T model);
    }
}
