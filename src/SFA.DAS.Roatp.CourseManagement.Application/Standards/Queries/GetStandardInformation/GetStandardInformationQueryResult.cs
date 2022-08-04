namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation
{
    public class GetStandardInformationQueryResult
    {
        public string StandardUId { get; set; }
        public string IfateReferenceNumber { get; set; }
        public int LarsCode { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public string Version { get; set; }
        public string RegulatorName { get; set; }
        public string Route { get; set; }
    }
}
