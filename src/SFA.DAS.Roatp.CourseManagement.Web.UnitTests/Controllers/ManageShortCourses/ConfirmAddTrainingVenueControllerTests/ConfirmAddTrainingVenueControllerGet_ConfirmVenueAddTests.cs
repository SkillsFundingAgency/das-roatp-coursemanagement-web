using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.ConfirmAddTrainingVenueControllerTests;
public class ConfirmAddTrainingVenueControllerGet_ConfirmVenueAddTests
{
    [Test, MoqAutoData]
    public void ConfirmVenueAdd_AddressInTempData_ReturnsViewResult(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        Mock<IUrlHelper> urlHelperMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        string cancelLinkUrl)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddTrainingVenue, cancelLinkUrl);

        // Act
        var result = sut.ConfirmVenueAdd(apprenticeshipType) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        result.ViewName.Should().Be(ConfirmAddTrainingVenueController.ViewPath);
        var model = result.Model as ConfirmAddTrainingVenueViewModel;
        model!.AddressLine1.Should().Be(addressItem.AddressLine1);
        model!.CancelLink.Should().Be(cancelLinkUrl);
        model!.Route.Should().Be(RouteNames.PostConfirmAddTrainingVenue);
        model!.IsAddJourney.Should().Be(true);
    }

    [Test]
    [MoqInlineAutoData(true, ButtonText.Confirm)]
    [MoqInlineAutoData(false, ButtonText.Continue)]
    public void ConfirmVenueAdd_HasSeenSummaryPageIsTrueOrFalse_ReturnsExpectedButtonText(
    bool hasSeenSummaryPage,
    string expectedSubmitButtonText,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    Mock<ITempDataDictionary> tempDataMock,
    Mock<IUrlHelper> urlHelperMock,
    [Greedy] ConfirmAddTrainingVenueController sut,
    ShortCourseSessionModel sessionModel,
    AddressItem addressItem,
    string cancelLinkUrl)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.HasSeenSummaryPage = hasSeenSummaryPage;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddTrainingVenue, cancelLinkUrl);

        // Act
        var result = sut.ConfirmVenueAdd(apprenticeshipType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddTrainingVenueViewModel;
        model.SubmitButtonText.Should().Be(expectedSubmitButtonText);
    }

    [Test]
    [MoqInlineAutoData(true, false)]
    [MoqInlineAutoData(false, true)]
    public void ConfirmVenueAdd_HasSeenSummaryPageIsTrueOrFalse_ShowCancelButtonSetsCorrectly(
        bool hasSeenSummaryPage,
        bool showCancelButton,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        Mock<IUrlHelper> urlHelperMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        string cancelLinkUrl)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.HasSeenSummaryPage = hasSeenSummaryPage;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddTrainingVenue, cancelLinkUrl);

        // Act
        var result = sut.ConfirmVenueAdd(apprenticeshipType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddTrainingVenueViewModel;
        model.ShowCancelOption.Should().Be(showCancelButton);
    }

    [Test, MoqAutoData]
    public void ConfirmVenueAdd_SessionIsNull_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.ConfirmVenueAdd(apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void ConfirmVenueAdd_LocationsAvailableIsTrueInSession_RedirectsToSelectShortCourseTrainingVenue(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.LocationsAvailable = true;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.ConfirmVenueAdd(apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.SelectShortCourseTrainingVenue);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void ConfirmVenueAdd_AddressNotInTempData_RedirectsToSelectShortCourseTrainingVenue(
    Mock<ITempDataDictionary> tempDataMock,
    [Greedy] ConfirmAddTrainingVenueController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        object address = null;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmVenueAdd(apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
