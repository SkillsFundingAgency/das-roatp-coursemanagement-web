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
public class ConfirmAddTrainingVenueControllerGetTests
{
    [Test]
    [MoqInlineAutoData("", RouteNames.PostConfirmAddTrainingVenue, true)]
    [MoqInlineAutoData("test", RouteNames.PostConfirmAddTrainingVenueEditShortCourse, false)]
    public void ConfirmVenue_AddressInTempData_ReturnsViewResult(
        string larsCode,
        string expectedPostRoute,
        bool expectedIsAddJourney,
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
        var result = sut.ConfirmVenue(apprenticeshipType, larsCode) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        result.ViewName.Should().Be(ConfirmAddTrainingVenueController.ViewPath);
        var model = result.Model as ConfirmAddTrainingVenueViewModel;
        model!.AddressLine1.Should().Be(addressItem.AddressLine1);
        model!.CancelLink.Should().Be(cancelLinkUrl);
        model!.Route.Should().Be(expectedPostRoute);
        model!.IsAddJourney.Should().Be(expectedIsAddJourney);
    }

    [Test]
    [MoqInlineAutoData("", true, ButtonText.Confirm)]
    [MoqInlineAutoData("test", false, ButtonText.Confirm)]
    public void ConfirmVenue_HasSeenSummaryPageIsTrueOrFalse_ReturnsExpectedButtonText(
        string larsCode,
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
        var result = sut.ConfirmVenue(apprenticeshipType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddTrainingVenueViewModel;
        model.SubmitButtonText.Should().Be(expectedSubmitButtonText);
    }

    [Test]
    [MoqInlineAutoData("", true, false)]
    [MoqInlineAutoData("test", false, false)]
    [MoqInlineAutoData("", false, true)]
    [MoqInlineAutoData("test", true, false)]
    public void ConfirmVenue_HasSeenSummaryPageIsTrueOrFalse_ShowCancelButtonSetsCorrectly(
        string larsCode,
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
        var result = sut.ConfirmVenue(apprenticeshipType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddTrainingVenueViewModel;
        model.ShowCancelOption.Should().Be(showCancelButton);
    }

    [Test, MoqAutoData]
    public void ConfirmVenue_AddressNotInTempData_RedirectsToEditShortCourseTrainingVenues(
        string larsCode,
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
        var result = sut.ConfirmVenue(apprenticeshipType, "test") as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.EditShortCourseTrainingVenues);
    }

    [Test, MoqAutoData]
    public void CancelAddTrainingVenue_RemovesTempData_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var ukprn = 12345;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        // Act
        var result = sut.CancelAddTrainingVenue(ukprn, apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
        result!.RouteName.Should().Be(RouteNames.SelectShortCourseLocationOption);
    }
}
