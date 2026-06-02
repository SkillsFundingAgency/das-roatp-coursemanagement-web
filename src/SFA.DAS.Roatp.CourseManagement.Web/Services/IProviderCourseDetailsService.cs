using System.Threading.Tasks;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services;

public interface IProviderCourseDetailsService
{
    Task<GetProviderCourseDetailsQueryResult> GetProviderCourseDetails(int ukprn, string larsCode);
}
