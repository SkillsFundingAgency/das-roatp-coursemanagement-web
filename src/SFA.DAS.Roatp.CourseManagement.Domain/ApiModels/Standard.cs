namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels
{
    public class Standard
    {
        public int ProviderCourseId { get; set; }
        public string CourseName { get; set; }
        public int Level { get; set; }
        public bool IsImported { get; set; }
        public string LarsCode { get; set; }
        public string Version { get; set; }
        public string ApprovalBody { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
        public bool IsRegulatedForProvider { get; set; }
        public bool HasLocations { get; set; }
    }
}
