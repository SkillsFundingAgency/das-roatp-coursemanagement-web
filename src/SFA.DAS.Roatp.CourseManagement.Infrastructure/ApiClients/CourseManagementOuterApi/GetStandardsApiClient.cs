using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Roatp.CourseManagement.Domain.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Infrastructure.ApiClients.CourseManagementOuterApi
{
    public class GetStandardsApiClient : ApiClientBase<GetStandardsApiClient>, IGetStandardsApiClient
    {
        public GetStandardsApiClient(HttpClient client, ILogger<GetStandardsApiClient> logger)
                    : base(client, logger)
        {
        }

        public async Task<List<Standard>> GetAllStandards(int ukprn)
        {
            var result = await Get<List<Standard>>($"/Standards/{ukprn}");
            return result;
        }
    }
}
