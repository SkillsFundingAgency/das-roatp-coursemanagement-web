using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels
{
    public class StandardDetails
    {
        public string CourseName { get; set; }
        public int Level { get; set; }
        public string IFateReferenceNumber { get; set; }
        public string Sector { get; set; }
        public string LarsCode { get; set; }
        public string RegulatorName { get; set; }
        public ApprenticeshipType ApprenticeshipType { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public List<ProviderCourseLocation> ProviderCourseLocations { get; set; } = new List<ProviderCourseLocation>();
        public bool? IsApprovedByRegulator { get; set; }
        public bool IsRegulatedForProvider { get; set; }
        public bool HasLocations { get; set; }
        public bool HasOnlineDeliveryOption { get; set; }
        public CourseType CourseType { get; set; }
        public int Duration { get; set; }
        public DurationUnits DurationUnits { get; set; }
    }
}