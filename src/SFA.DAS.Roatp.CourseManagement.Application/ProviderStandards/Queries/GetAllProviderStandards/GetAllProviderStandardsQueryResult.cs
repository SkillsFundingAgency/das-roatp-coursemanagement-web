using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;

public class GetAllProviderStandardsQueryResult
{
    public List<Standard> Standards { get; set; } = [];
}
