using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses;
public class ShortCourseViewModelTests
{
    [Test, MoqAutoData]
    public void ShortCourseViewModel_ImplicitOperator_ConvertsFromStandard(
        Standard standard)
    {
        // Arrange
        standard.CourseName = "course name";
        standard.Level = 1;

        var expectedCourseDisplayName = "course name (level 1)";

        // Act
        ShortCourseViewModel viewModel = standard;

        // Assert
        viewModel.ProviderCourseId.Should().Be(standard.ProviderCourseId);
        viewModel.CourseName.Should().Be(standard.CourseName);
        viewModel.Level.Should().Be(standard.Level);
        viewModel.LarsCode.Should().Be(standard.LarsCode);
        viewModel.CourseDisplayName.Should().Be(expectedCourseDisplayName);
        viewModel.HasLocation.Should().Be(standard.HasLocations);
    }
}
