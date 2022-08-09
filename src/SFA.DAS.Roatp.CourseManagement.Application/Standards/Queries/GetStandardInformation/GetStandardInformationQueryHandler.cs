using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation
{
    public class GetStandardInformationQueryHandler : IRequestHandler<GetStandardInformationQuery, GetStandardInformationQueryResult>
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<GetStandardInformationQueryHandler> _logger;

        public GetStandardInformationQueryHandler(IApiClient apiClient, ILogger<GetStandardInformationQueryHandler> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<GetStandardInformationQueryResult> Handle(GetStandardInformationQuery request, CancellationToken cancellationToken)
        {
            var result = await _apiClient.Get<GetStandardInformationQueryResult>($"lookup/standards/{request.LarsCode}");

            if (result == null)
            {
                _logger.LogError("Failed to get standard information for larscode:{larscode}", request.LarsCode);
                throw new InvalidOperationException($"Standard information not found for larscode: {request.LarsCode}");
            }

            return result;
        }
    }
}
