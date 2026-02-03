using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses;
public class ShortCourseBaseViewModelTests
{
    [Test]
    public void CourseTypeLower_IsHumanizedLowerCase()
    {
        var sut = new ShortCourseBaseViewModel()
        {
            CourseType = CourseType.ApprenticeshipUnit,
        };

        sut.CourseTypeLower.Should().Be("apprenticeship unit");
    }

    [Test]
    public void CourseTypeLowerPlural_IsHumanizedLowerCasePlural()
    {
        var sut = new ShortCourseBaseViewModel()
        {
            CourseType = CourseType.ApprenticeshipUnit,
        };

        sut.CourseTypeLowerPlural.Should().Be("apprenticeship units");
    }

    [Test]
    public void NullCheck_CourseTypeIsNull_ReturnsNull()
    {
        var sut = new ShortCourseBaseViewModel()
        {
            CourseType = null
        };

        sut.CourseTypeLower.Should().BeNull();
        sut.CourseTypeLowerPlural.Should().BeNull();
    }
}
