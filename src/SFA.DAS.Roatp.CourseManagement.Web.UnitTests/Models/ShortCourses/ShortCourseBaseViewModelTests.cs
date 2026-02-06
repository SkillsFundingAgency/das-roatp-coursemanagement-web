using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses;
public class ShortCourseBaseViewModelTests
{
    [Test]
    public void ApprenticeshipTypeLower_IsHumanizedLowerCase()
    {
        var sut = new ShortCourseBaseViewModel()
        {
            ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit,
        };

        sut.ApprenticeshipTypeLower.Should().Be("apprenticeship unit");
    }

    [Test]
    public void ApprenticeshipTypeLowerPlural_IsHumanizedLowerCasePlural()
    {
        var sut = new ShortCourseBaseViewModel()
        {
            ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit,
        };

        sut.ApprenticeshipTypeLowerPlural.Should().Be("apprenticeship units");
    }

    [Test]
    public void NullCheck_ApprenticeshipTypeIsNull_ReturnsNull()
    {
        var sut = new ShortCourseBaseViewModel()
        {
            ApprenticeshipType = null
        };

        sut.ApprenticeshipTypeLower.Should().BeNull();
        sut.ApprenticeshipTypeLowerPlural.Should().BeNull();
    }
}
