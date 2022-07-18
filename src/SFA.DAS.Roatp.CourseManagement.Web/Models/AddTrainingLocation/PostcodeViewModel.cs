namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation
{
    public class PostcodeViewModel : PostcodeSubmitModel
    {
        public string BackLink { get; set; } = "#";
        public string CancelLink { get; set; } = "#";
    }
    public class PostcodeSubmitModel
    {
        public string Postcode { get; set; }
    }
}
