namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels
{
    public class CourseRegionModel
    {
        public int Id { get; set; }
        public string SubregionName { get; set; }
        public string RegionName { get; set; }
        public bool IsSelected { get; set; }
    }
}
