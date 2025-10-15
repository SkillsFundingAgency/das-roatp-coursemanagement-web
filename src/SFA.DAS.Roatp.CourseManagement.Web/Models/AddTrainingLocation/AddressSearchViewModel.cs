namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

public class AddressSearchViewModel : AddressSearchSubmitModel, IBackLink
{
}

public class AddressSearchSubmitModel
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
