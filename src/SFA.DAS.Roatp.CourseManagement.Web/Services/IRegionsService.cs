using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services;

public interface IRegionsService
{
    Task<List<RegionModel>> GetRegions();
}
