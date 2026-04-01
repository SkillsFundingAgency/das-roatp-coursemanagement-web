using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ShortCourseTrainingVenuesViewModel : ShortCourseTrainingVenuesSubmitModel, IBackLink
{
    public List<TrainingVenueModel> TrainingVenues { get; set; } = new List<TrainingVenueModel>();
    public string SubmitButtonText { get; set; }
    public string Route { get; set; }
    public bool IsAddJourney { get; set; }
}