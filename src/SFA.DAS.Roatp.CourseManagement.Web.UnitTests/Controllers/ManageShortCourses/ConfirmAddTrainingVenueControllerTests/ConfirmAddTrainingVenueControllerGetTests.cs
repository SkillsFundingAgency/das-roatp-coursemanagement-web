using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Text.Json;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.ConfirmAddTrainingVenueControllerTests;
public class ConfirmAddTrainingVenueControllerGetTests
{
    [MoqInlineAutoData("")]
    [MoqInlineAutoData("test")]
    public void ConfirmVenue_AddressInTempData_ReturnsViewResult(
        string larsCode,
        Mock<ITempDataDictionary> tempDataMock,
        Mock<IUrlHelper> urlHelperMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem,
        string cancelLinkUrl)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out serialisedAddressItem));

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddTrainingVenue, cancelLinkUrl);

        // Act
        var result = sut.ConfirmVenue(courseType, larsCode) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        result.ViewName.Should().Be(ConfirmAddTrainingVenueController.ViewPath);
        var model = result.Model as ConfirmAddTrainingVenueViewModel;
        model!.AddressLine1.Should().Be(addressItem.AddressLine1);
        model!.CancelLink.Should().Be(cancelLinkUrl);
    }

    [MoqInlineAutoData("")]
    [MoqInlineAutoData("test")]
    public void ConfirmVenue_AddressNotInTempData_RedirectsToSelectShortCourseTrainingVenue(
        string larsCode,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddTrainingVenueController sut)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;
        object address = null;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmVenue(courseType, "") as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public void ConfirmVenue_IsAddJourney_SessionIsNull_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.ConfirmVenue(courseType, "") as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void ConfirmVenue_IsAddJourney_LocationsAvailableIsTrueInSession_RedirectsToSelectShortCourseTrainingVenue(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;
        sessionModel.LocationsAvailable = true;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.ConfirmVenue(courseType, "") as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.SelectShortCourseTrainingVenue);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void CancelAddTrainingVenue_RemovesTempData_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;
        var ukprn = 12345;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out serialisedAddressItem));

        // Act
        var result = sut.CancelAddTrainingVenue(ukprn, courseType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedAddressTempDataKey));
        result!.RouteName.Should().Be(RouteNames.SelectShortCourseLocationOption);
    }
}
