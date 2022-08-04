namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardInformationViewModel
    {
        public int LarsCode { get; set; }
        public string CourseName { get; set; }
        public int Level { get; set; }
        public string IFateReferenceNumber { get; set; }
        public string Sector { get; set; }
        public string RegulatorName { get; set; }
        public string Version { get; set; }
        public string CourseDisplayName => $"{CourseName} (Level {Level})";
        public bool IsStandardRegulated => !string.IsNullOrEmpty(RegulatorName);
    }
}
