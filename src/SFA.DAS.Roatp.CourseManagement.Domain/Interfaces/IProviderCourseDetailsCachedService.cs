using System.Threading.Tasks;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

public interface IProviderCourseDetailsCachedService
{
    Task<StandardDetails> GetCachedProviderCourseDetails(int ukprn, string larsCode);
}