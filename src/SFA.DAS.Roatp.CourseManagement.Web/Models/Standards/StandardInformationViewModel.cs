using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

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
        public ApprenticeshipType ApprenticeshipType { get; set; }
        public string CourseDisplayName => $"{CourseName} (level {Level})";
        public bool IsStandardRegulated => !string.IsNullOrEmpty(RegulatorName);
        public bool IsRegulatedForProvider { get; set; }

        public static implicit operator StandardInformationViewModel(GetStandardInformationQueryResult source)
            => new()
            {
                LarsCode = source.LarsCode,
                IfateReferenceNumber = source.IfateReferenceNumber,
                ApprenticeshipType = source.ApprenticeshipType,
                Sector = source.Sector,
                CourseName = source.Title,
                Level = source.Level,
                RegulatorName = source.RegulatorName,
                IsRegulatedForProvider = source.IsRegulatedForProvider
            };
    }
}
