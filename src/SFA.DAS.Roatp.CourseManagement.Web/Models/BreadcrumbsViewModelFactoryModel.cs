using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public class BreadcrumbsViewModelFactoryModel
{
    public int? Ukprn { get; init; }
    public LearningType? LearningType { get; init; }
    public IEnumerable<string> BreadcrumbNames { get; init; } = [];
}
