using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class SelectShortCourseTrainingVenuesViewModel : SelectShortCourseTrainingVenuesSubmitModel, IBackLink
{
    public List<TrainingVenueModel> TrainingVenues { get; set; } = new List<TrainingVenueModel>();
}