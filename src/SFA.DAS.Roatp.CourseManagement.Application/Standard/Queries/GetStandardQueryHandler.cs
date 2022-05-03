﻿using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries
{
    public class GetStandardQueryHandler : IRequestHandler<GetStandardQuery, GetStandardQueryResult>
    {
        private readonly ILogger<GetStandardQueryHandler> _logger;
        private readonly IGetStandardsApiClient _getStandardApiClient;
        public GetStandardQueryHandler(IGetStandardsApiClient getStandardApiClient, ILogger<GetStandardQueryHandler> logger)
        {
            _logger = logger;
            _getStandardApiClient = getStandardApiClient;
        }
        public async Task<GetStandardQueryResult> Handle(GetStandardQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get Standards request received for Ukprn number {Ukprn}", request.Ukprn);
            try
            {
                var standards = await _getStandardApiClient.GetAllStandards(request.Ukprn);
                if (standards == null)
                {
                    _logger.LogInformation("Courses data not found for {ukprn}", request.Ukprn);
                    return null;
                }

                return new GetStandardQueryResult
                {
                    Standards = standards
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred trying to retrieve Standards for Ukprn {request.Ukprn}");
                throw;
            }
        }
    }
}