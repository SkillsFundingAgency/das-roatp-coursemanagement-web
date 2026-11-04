using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

public class ProviderCourseLocationRemoveViewModel : ProviderCourseLocationRemoveSubmitModel, IBackLink
{
    public DeliveryMethodModel DeliveryMethod { get; set; } = new();
    public string LocationName { get; set; }
}

public class ProviderCourseLocationRemoveSubmitModel
{
    public required int Ukprn { get; set; }
    public required string LarsCode { get; set; }
    public required Guid Id { get; set; }
    public bool IsOnlyLocation { get; set; } = false;
}
