using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation
{
    public class GetStandardInformationQueryResult
    {
        public string StandardUId { get; set; }
        public string IfateReferenceNumber { get; set; }
        public int LarsCode { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public ApprenticeshipType ApprenticeshipType { get; set; }
        public string RegulatorName { get; set; }
        public string Sector { get; set; }
        public bool IsRegulatedForProvider { get; set; }
    }
}
