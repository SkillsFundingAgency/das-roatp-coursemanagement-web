using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

public class ProviderLocationNotDeletedViewModel : IBackLink
{
    public string LocationName { get; set; }
    public List<LocationStandardModel> StandardsWithoutOtherVenues { get; set; } = new();
    public string BackUrl { get; set; }

    public static implicit operator ProviderLocationNotDeletedViewModel(ProviderLocation source)
    {
        var standards = source.Standards.Select(s => s)
            .Where(s => !s.HasOtherVenues).OrderBy(s => s.Title)
            .ThenBy(s => s.Level).ToList();

        foreach (var standard in standards)
        {
            standard.CourseDisplayName = standard.Title + " (level " + standard.Level + ")";
        }

        return new ProviderLocationNotDeletedViewModel
        {
            LocationName = source.LocationName,
            StandardsWithoutOtherVenues = standards
        };
    }
}
