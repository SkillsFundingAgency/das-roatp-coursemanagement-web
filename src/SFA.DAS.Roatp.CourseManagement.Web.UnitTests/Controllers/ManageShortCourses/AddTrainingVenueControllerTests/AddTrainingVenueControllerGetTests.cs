using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.AddTrainingVenueControllerTests;
public class AddTrainingVenueControllerGetTests
{
    [Test, MoqAutoData]
    public void LookupAddress_ReturnsExpectedView(
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddTrainingVenueController sut)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        // Act
        var addressSearch = sut.LookupAddress(courseType);

        // Assert
        addressSearch.Result.As<ViewResult>().Should().NotBeNull();
        addressSearch.Result.As<ViewResult>().ViewName.Should().Be(AddTrainingVenueController.ViewPath);
    }
}
