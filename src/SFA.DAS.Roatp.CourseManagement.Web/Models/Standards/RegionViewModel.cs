using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class RegionViewModel
    {
        public int Id { get; set; }
        public string SubregionName { get; set; }
        public string RegionName { get; set; }
        public bool IsSelected { get; set; }

        public static implicit operator RegionViewModel(CourseRegionModel source)
        {
            return new RegionViewModel
            {
                Id = source.Id,
                SubregionName = source.SubregionName,
                RegionName = source.RegionName,
                IsSelected = source.IsSelected
            };
        }

        public static implicit operator RegionViewModel(RegionModel source)
            => new RegionViewModel
            {
                Id = source.Id,
                SubregionName = source.SubregionName,
                RegionName = source.RegionName,
            };

    }
}
