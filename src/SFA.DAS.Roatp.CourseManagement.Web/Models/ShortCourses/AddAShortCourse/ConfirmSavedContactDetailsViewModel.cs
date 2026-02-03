using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class ConfirmSavedContactDetailsViewModel : ConfirmSavedContactDetailsSubmitModel, IBackLink
{
    [FromRoute]
    public required int Ukprn { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public bool? ShowEmail { get; set; }
    public bool? ShowPhone { get; set; }
    public required CourseType CourseType { get; set; }
}

public class ConfirmSavedContactDetailsSubmitModel
{
    public bool? IsUsingSavedContactDetails { get; set; }
}
