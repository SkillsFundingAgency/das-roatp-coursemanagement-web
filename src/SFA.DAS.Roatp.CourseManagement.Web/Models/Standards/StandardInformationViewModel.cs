using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardInformationViewModel
    {
        public int LarsCode { get; set; }
        public string CourseName { get; set; }
        public int Level { get; set; }
        public string IfateReferenceNumber { get; set; }
        public string Sector { get; set; }
        public string RegulatorName { get; set; }
        public string Version { get; set; }
        public string CourseDisplayName => $"{CourseName} (Level {Level})";
        public bool IsStandardRegulated => !string.IsNullOrEmpty(RegulatorName);

        public static implicit operator StandardInformationViewModel(GetStandardInformationQueryResult source)
            => new StandardInformationViewModel
            {
                LarsCode = source.LarsCode,
                IfateReferenceNumber = source.IfateReferenceNumber,
                Version = source.Version,
                Sector = source.Route,
                CourseName = source.Title,
                Level = source.Level,
                RegulatorName = source.RegulatorName
            };
    }
}
