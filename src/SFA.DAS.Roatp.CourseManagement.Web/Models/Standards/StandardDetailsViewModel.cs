using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

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
        public bool IsStandardRegulated => !string.IsNullOrEmpty(RegulatorName);
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }
        public string BackUrl { get; set; }

        public static implicit operator StandardDetailsViewModel(StandardDetails standardDetails)
        {
            return new StandardDetailsViewModel
            {
                CourseName = standardDetails.CourseName,
                Level = standardDetails.Level,
                IFateReferenceNumber = standardDetails.IFateReferenceNumber,
                LarsCode = standardDetails.LarsCode,
                RegulatorName = standardDetails.RegulatorName,
                Sector = standardDetails.Sector,
                Version = standardDetails.Version,
                StandardInfoUrl = standardDetails.StandardInfoUrl,
                ContactUsPhoneNumber = standardDetails.ContactUsPhoneNumber,
                ContactUsEmail = standardDetails.ContactUsEmail,
                ContactUsPageUrl = standardDetails.ContactUsPageUrl,
            };
        }
    }
}
