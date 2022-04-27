using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.ApiClients
{
    public class RoatpCourseManagementOuterApiClient : ApiClientBase<RoatpCourseManagementOuterApiClient>, IRoatpCourseManagementOuterApiClient
    {
        public RoatpCourseManagementOuterApiClient(HttpClient client, ILogger<RoatpCourseManagementOuterApiClient> logger)
                    : base(client, logger)
        {
        }

        public async Task<StandardsListViewModel> GetAllStandards(int ukprn)
        {
            var result = await Get<List<StandardsViewModel>>($"/Standards/{ukprn}");
            var response = new StandardsListViewModel
            {
                Standards = result
            };
            return response;
        }
    }
}
