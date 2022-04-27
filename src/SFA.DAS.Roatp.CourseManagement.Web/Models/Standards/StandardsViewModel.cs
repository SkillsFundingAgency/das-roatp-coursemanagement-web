namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardsViewModel
    {
        public int ProviderCourseId { get; set; }
        public string CourseName { get; set; }
        public int Level { get; set; }
        public bool IsImported { get; set; } = false;
    }
}
