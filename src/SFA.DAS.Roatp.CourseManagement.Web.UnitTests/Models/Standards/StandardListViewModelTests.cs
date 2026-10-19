using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards;

public class StandardListViewModelTests
{
    [TestCase(true, true, true, 0)]
    [TestCase(false, true, true, 1)]
    [TestCase(false, false, true, 2)]
    public void WhenModelCreated_ThenCoursesMissingInfoCountIsSet(
        bool firstHasLocation,
        bool secondHasLocation,
        bool thirdHasLocation,
        int expectedCount)
    {
        var standards = new List<StandardViewModel>
        {
            new() { CourseName = "Test 1", HasLocation = firstHasLocation },
            new() { CourseName = "Test 2", HasLocation = secondHasLocation },
            new() { CourseName = "Test 3", HasLocation = thirdHasLocation }
        };

        var sut = new StandardListViewModel { Standards = standards };
        sut.SetCourseValues();

        sut.CoursesMissingInfoCount.Should().Be(expectedCount);
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    public void WhenModelCreated_ThenShowCourseMissingInfoIsSet(
        bool hasLocation,
        bool expected)
    {
        var standards = new List<StandardViewModel>
        {
            new() { CourseName = "Test", HasLocation = hasLocation }
        };

        var sut = new StandardListViewModel { Standards = standards };
        sut.SetCourseValues();

        sut.ShowCourseMissingInfo.Should().Be(expected);
    }

    [TestCase(true, true, true, false)]
    [TestCase(false, true, true, true)]
    [TestCase(false, false, true, false)]
    public void WhenModelCreated_ThenShowSingleCourseMissingInfoIsSet(
        bool firstHasLocation,
        bool secondHasLocation,
        bool thirdHasLocation,
        bool expected)
    {
        var standards = new List<StandardViewModel>
        {
            new() { CourseName = "Test 1", HasLocation = firstHasLocation },
            new() { CourseName = "Test 2", HasLocation = secondHasLocation },
            new() { CourseName = "Test 3", HasLocation = thirdHasLocation }
        };

        var sut = new StandardListViewModel { Standards = standards };
        sut.SetCourseValues();

        sut.ShowSingleCourseMissingInfo.Should().Be(expected);
    }

    [TestCase(true, true, false)]
    [TestCase(false, true, false)]
    [TestCase(false, false, true)]
    public void WhenModelCreated_ThenShowListOfCoursesMissingInfoIsSet(
        bool firstHasLocation,
        bool secondHasLocation,
        bool expected)
    {
        var standards = new List<StandardViewModel>
        {
            new() { CourseName = "Test 1", HasLocation = firstHasLocation },
            new() { CourseName = "Test 2", HasLocation = secondHasLocation }
        };

        var sut = new StandardListViewModel { Standards = standards };
        sut.SetCourseValues();

        sut.ShowListOfCoursesMissingInfo.Should().Be(expected);
    }

    [Test]
    public void WhenModelCreated_ThenTotalCourseCountIsSet()
    {
        var standards = new List<StandardViewModel>
        {
            new()
            {
                CourseName = "Test",
                HasLocation = true
            }
        };

        var sut = new StandardListViewModel { Standards = standards };
        sut.SetCourseValues();

        sut.TotalCourseCount.Should().Be(1);
    }

    [Test]
    public void WhenModelCreated_AndHasCourses_ThenHasCoursesFlagsAreSet()
    {
        var standards = new List<StandardViewModel>
        {
            new()
            {
                CourseName = "Test",
                HasLocation = true
            }
        };

        var sut = new StandardListViewModel { Standards = standards };
        sut.SetCourseValues();

        sut.HasCourses.Should().BeTrue();
        sut.HasNoCourses.Should().BeFalse();
    }

    [Test]
    public void WhenModelCreated_AndDosentHaveCourses_ThenHasCoursesFlagsAreSet()
    {
        var sut = new StandardListViewModel
        {
            Standards = new List<StandardViewModel>()
        };
        sut.SetCourseValues();

        sut.HasCourses.Should().BeFalse();
        sut.HasNoCourses.Should().BeTrue();
    }
}
