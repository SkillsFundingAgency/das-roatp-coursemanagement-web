using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services;

public interface IProviderCourseTypeService
{
    Task<List<CourseTypeModel>> GetProviderCourseType(int ukprn);
}
