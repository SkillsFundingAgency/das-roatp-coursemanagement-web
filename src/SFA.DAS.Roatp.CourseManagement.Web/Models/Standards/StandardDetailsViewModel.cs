namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardDetailsViewModel
    {
        public int LarsCode { get; set; }
        public string CourseName { get; set; }
        public string Level { get; set; }
        public string CourseDisplayName => $"{CourseName} (Level {Level})";
        public string IFateReferenceNumber { get; set; }
        public string Sector { get; set; }
        public string RegulatorName { get; set; }
        public string Version { get; set; }
        public bool IsRegulatorPresent => !string.IsNullOrEmpty(RegulatorName);
        public string BackUrl { get; set; }
    }
}
