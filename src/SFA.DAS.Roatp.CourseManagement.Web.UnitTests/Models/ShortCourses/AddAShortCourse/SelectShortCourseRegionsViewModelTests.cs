using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.AddAShortCourse;
public class SelectShortCourseRegionsViewModelTests
{
    private const string Region1 = "Region1";
    private const string Region2 = "Region2";
    [Test, AutoData]
    public void GetGroupedSubRegions_ReturnsGroupedSubRegions()
    {
        var fixture = new Fixture();
        var region1List = fixture.Build<ShortCourseRegionViewModel>().With(r => r.RegionName, Region1).CreateMany(4).ToList();
        var region2List = fixture.Build<ShortCourseRegionViewModel>().With(r => r.RegionName, Region2).CreateMany(2);

        region1List.AddRange(region2List);
        var model = new SelectShortCourseRegionsViewModel(region1List);

        model.SubregionsGroupedByRegions.Count().Should().Be(2);
        model.SubregionsGroupedByRegions.First(g => g.Key == Region1).Count().Should().Be(4);
        model.SubregionsGroupedByRegions.First(g => g.Key == Region2).Count().Should().Be(2);
    }

    [Test]
    public void SubRegionsGroupedByRegions_OrderedByRegionName_ReturnsInAlphabeticalOrder()
    {
        var fixture = new Fixture();
        var list = new List<ShortCourseRegionViewModel>();
        list.AddRange(fixture.Build<ShortCourseRegionViewModel>().With(r => r.RegionName, "B_Region").CreateMany(2));
        list.AddRange(fixture.Build<ShortCourseRegionViewModel>().With(r => r.RegionName, "A_Region").CreateMany(1));
        list.AddRange(fixture.Build<ShortCourseRegionViewModel>().With(r => r.RegionName, "C_Region").CreateMany(3));

        var model = new SelectShortCourseRegionsViewModel(list);

        var keys = model.SubregionsGroupedByRegions.Select(g => g.Key).ToList();

        keys.Should().BeInAscendingOrder();
    }
}
