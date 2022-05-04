using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Domain.Interfaces
{
    public interface IGetStandardsApiClient
    {
        Task<List<Standard>> GetAllStandards(int ukprn);
    }
}
