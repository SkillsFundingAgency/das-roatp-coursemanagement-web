using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.ManageShortCourses;

public class ManageShortCoursesViewModelTests
{
    [Test]
    public void WhenModelCreated_ThenTotalCourseCountIsSet()
    {
        var shortCourses = new List<ShortCourseViewModel>
        {
            new()
            {
                CourseName = "Test",
                HasLocation = true
            }
        };

        var sut = new ManageShortCoursesViewModel { ShortCourses = shortCourses };
        sut.SetCourseValues();

        sut.TotalCourseCount.Should().Be(1);
    }

    [Test]
    public void WhenModelCreated_AndHasCourses_ThenHasCoursesFlagsAreSet()
    {
        var shortCourses = new List<ShortCourseViewModel>
        {
            new()
            {
                CourseName = "Test",
                HasLocation = true
            }
        };

        var sut = new ManageShortCoursesViewModel { ShortCourses = shortCourses };
        sut.SetCourseValues();

        sut.HasCourses.Should().BeTrue();
        sut.HasNoCourses.Should().BeFalse();
    }

    [Test]
    public void WhenModelCreated_AndDosentHaveCourses_ThenHasCoursesFlagsAreSet()
    {
        var sut = new ManageShortCoursesViewModel
        {
            ShortCourses = new List<ShortCourseViewModel>()
        };
        sut.SetCourseValues();

        sut.HasCourses.Should().BeFalse();
        sut.HasNoCourses.Should().BeTrue();
    }
}
