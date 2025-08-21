using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services;

public static class StandardDescriptionListService
{
    public static List<string> BuildSelectedStandardsList(List<ProviderContactStandardModel> standards)
    {
        List<string> checkedStandards = new();

        foreach (var standard in standards.OrderBy(s => s.CourseName).ThenBy(s => s.Level)
                     .Where(s => s.IsSelected))
        {
            checkedStandards.Add(standard.CourseName + " (Level " + standard.Level + ")");
        }

        return checkedStandards;
    }
}
