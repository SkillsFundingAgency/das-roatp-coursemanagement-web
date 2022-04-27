using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.ApiClients
{
    public interface IRoatpCourseManagementOuterApiClient
    {
        Task<StandardsListViewModel> GetAllStandards(int ukprn);
    }
}
