﻿using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllStandardRegions
{
    public class GetAllStandardRegionsQueryResult
    {
        public List<Domain.ApiModels.Region> Regions { get; set; } = new List<Domain.ApiModels.Region>();
    }
}
