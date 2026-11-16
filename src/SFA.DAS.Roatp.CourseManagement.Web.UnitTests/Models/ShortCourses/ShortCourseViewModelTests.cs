using System;
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
        viewModel.HasOnlineDeliveryOption.Should().Be(standard.HasOnlineDeliveryOption);
        viewModel.LastDateStarts.Should().Be(standard.LastDateStarts);
    }

    [TestCase(true, false, false)]
    [TestCase(true, false, false)]
    [TestCase(false, false, true)]
    public void ShortCourseViewModel_SetMissingInfo_ReturnsExpectedResult(
        bool hasLocation,
        bool hasOnlineDelivery,
        bool expectedResult)
    {
        // Arrange
        var viewModel = new ShortCourseViewModel()
        {
            HasLocation = hasLocation,
            HasOnlineDeliveryOption = hasOnlineDelivery
        };

        // Act
        var result = viewModel.StandardRequiresMoreInfo;

        // Assert
        result.Should().Be(expectedResult);
    }

    [TestCase(false, false)]
    [TestCase(true, true)]
    public void ShortCourseViewModel_SetsHasLastDateStarts(
           bool hasLastDateStarts,
           bool expected)
    {
        var today = DateTime.UtcNow.Date;
        var viewModel = new ShortCourseViewModel
        {
            LastDateStarts = hasLastDateStarts ? today : null
        };

        viewModel.HasLastDateStarts.Should().Be(expected);
    }

    [TestCase(false, 0, false)]
    [TestCase(true, -1, false)]
    [TestCase(true, 0, true)]
    [TestCase(true, 1, true)]
    public void ShortCourseViewModel_SetsFundingClosing(
        bool hasLastDateStarts,
        int dayOffset,
        bool expected)
    {
        var today = DateTime.UtcNow.Date;
        var viewModel = new ShortCourseViewModel
        {
            LastDateStarts = hasLastDateStarts ? today.AddDays(dayOffset) : null
        };

        viewModel.FundingClosing.Should().Be(expected);
    }

    [TestCase(false, 0, false)]
    [TestCase(true, -1, true)]
    [TestCase(true, 0, false)]
    [TestCase(true, 1, false)]
    public void ShortCourseViewModel_SetsFundingClosed(
        bool hasLastDateStarts,
        int dayOffset,
        bool expected)
    {
        var today = DateTime.UtcNow.Date;
        var viewModel = new ShortCourseViewModel
        {
            LastDateStarts = hasLastDateStarts ? today.AddDays(dayOffset) : null
        };

        viewModel.FundingClosed.Should().Be(expected);
    }

    [TestCase(false, 0, "")]
    [TestCase(true, 1, "govuk-tag govuk-tag--orange")]
    [TestCase(true, 0, "govuk-tag govuk-tag--orange")]
    [TestCase(true, -1, "govuk-tag govuk-tag--grey")]
    public void ShortCourseViewModel_SetsTagColour(
        bool hasLastDateStarts,
        int dayOffset,
        string expected)
    {
        var today = DateTime.UtcNow.Date;
        var viewModel = new ShortCourseViewModel
        {
            LastDateStarts = hasLastDateStarts ? today.AddDays(dayOffset) : null
        };

        viewModel.TagColour.Should().Be(expected);
    }
}
