using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    [TestFixture]
    public class RegionsViewModelTests
    {
        private const string Region1 = "Region1";
        private const string Region2 = "Region2";
        [Test, AutoData]
        public void GetGroupedSubRegions_ReturnsGroupedSubRegions()
        {
            var fixture = new Fixture();
            var region1List = fixture.Build<RegionViewModel>().With(r => r.RegionName, Region1).CreateMany(4).ToList();
            var region2List = fixture.Build<RegionViewModel>().With(r => r.RegionName, Region2).CreateMany(2);

            var model = new RegionsViewModel();
            model.AllRegions = region1List;
            model.AllRegions.AddRange(region2List);

            model.GetGroupedSubRegions().Count().Should().Be(2);
            model.GetGroupedSubRegions().First(g => g.Key == Region1).Count().Should().Be(4);
            model.GetGroupedSubRegions().First(g => g.Key == Region2).Count().Should().Be(2);
        }
    }
}
