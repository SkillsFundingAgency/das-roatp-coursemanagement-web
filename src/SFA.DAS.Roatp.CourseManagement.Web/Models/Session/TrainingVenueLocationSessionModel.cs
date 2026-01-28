using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Session;

public class TrainingVenueLocationSessionModel
{
    public LocationType LocationType { get; set; }
    public Guid? ProviderLocationId { get; set; }
    public string LocationName { get; set; }

    public static implicit operator TrainingVenueLocationSessionModel(ProviderLocation source)
            => new TrainingVenueLocationSessionModel
            {
                LocationType = source.LocationType,
                ProviderLocationId = source.NavigationId,
                LocationName = source.LocationName
            };
}
