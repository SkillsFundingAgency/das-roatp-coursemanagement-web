namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

public class AddTrainingVenueViewModel : AddTrainingVenueSubmitModel, IBackLink
{
    public string SubmitButtonText { get; set; }
    public string Route { get; set; }
    public bool IsAddJourney { get; set; }
    public string DisplayHeader { get; set; }
}

public class AddTrainingVenueSubmitModel : ShortCourseBaseViewModel
{
    public string SearchTerm { get; set; }
    public string Town { get; set; }
    public string County { get; set; }
    public string Postcode { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }

}