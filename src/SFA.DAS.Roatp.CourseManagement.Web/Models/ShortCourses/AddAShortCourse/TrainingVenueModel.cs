using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class TrainingVenueModel
{
    public LocationType LocationType { get; set; }
    public Guid ProviderLocationId { get; set; }
    public string LocationName { get; set; }
    public bool IsSelected { get; set; }

    public static implicit operator TrainingVenueModel(ProviderLocation source)
            => new TrainingVenueModel
            {
                LocationType = source.LocationType,
                ProviderLocationId = source.NavigationId,
                LocationName = source.LocationName
            };
}
