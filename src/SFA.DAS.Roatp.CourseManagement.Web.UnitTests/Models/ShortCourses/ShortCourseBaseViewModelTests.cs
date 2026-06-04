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
            LearningType = LearningType.ApprenticeshipUnit,
        };

        sut.LearningTypeLower.Should().Be("apprenticeship unit");
    }

    [Test]
    public void ApprenticeshipTypeLowerPlural_IsHumanizedLowerCasePlural()
    {
        var sut = new ShortCourseBaseViewModel()
        {
            LearningType = LearningType.ApprenticeshipUnit,
        };

        sut.LearningTypeLowerPlural.Should().Be("apprenticeship units");
    }

    [Test]
    public void ApprenticeshipTypeHumanize_IsHumanized()
    {
        var sut = new ShortCourseBaseViewModel()
        {
            LearningType = LearningType.ApprenticeshipUnit,
        };

        sut.LearningTypeHumanize.Should().Be("Apprenticeship unit");
    }

    [Test]
    public void NullCheck_ApprenticeshipTypeIsNull_ReturnsNull()
    {
        var sut = new ShortCourseBaseViewModel()
        {
            LearningType = null
        };

        sut.LearningTypeLower.Should().BeNull();
        sut.LearningTypeLowerPlural.Should().BeNull();
        sut.LearningTypeHumanize.Should().BeNull();
    }
}
