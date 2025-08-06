using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels
{
    public class StandardDetails
    {
        public string CourseName { get; set; }
        public int Level { get; set; }
        public string IFateReferenceNumber { get; set; }
        public string Sector { get; set; }
        public int LarsCode { get; set; }
        public string RegulatorName { get; set; }
        public ApprenticeshipType ApprenticeshipType { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }
        public List<ProviderCourseLocation> ProviderCourseLocations { get; set; } = new List<ProviderCourseLocation>();
        public bool? IsApprovedByRegulator { get; set; }
        public bool IsRegulatedForProvider { get; set; }
        public bool HasLocations { get; set; }
    }
}