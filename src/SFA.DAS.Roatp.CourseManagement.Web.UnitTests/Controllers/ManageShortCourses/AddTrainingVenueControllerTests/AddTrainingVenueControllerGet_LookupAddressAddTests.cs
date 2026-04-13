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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.AddTrainingVenueControllerTests;
public class AddTrainingVenueControllerGet_LookupAddressAddTests
{
    [Test, MoqAutoData]
    public void LookupAddressAdd_ReturnsExpectedView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddTrainingVenueController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var addressSearch = sut.LookupAddressAdd(apprenticeshipType) as ViewResult;

        // Assert
        addressSearch.Should().NotBeNull();
        addressSearch.ViewName.Should().Be(AddTrainingVenueController.ViewPath);
        var model = addressSearch.Model as AddTrainingVenueViewModel;
        model.Route.Should().Be(RouteNames.PostAddTrainingVenue);
        model.IsAddJourney.Should().Be(true);
    }

    [Test]
    [MoqInlineAutoData(true, ButtonText.Confirm)]
    [MoqInlineAutoData(false, ButtonText.Continue)]
    public void LookupAddressAdd_HasSeenSummaryPageIsTrueOrFalse_ReturnsExpectedButtonText(
    bool hasSeenSummaryPage,
    string expectedSubmitButtonText,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<ITempDataDictionary> tempDataMock,
    [Greedy] AddTrainingVenueController sut,
    ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.HasSeenSummaryPage = hasSeenSummaryPage;

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var addressSearch = sut.LookupAddressAdd(apprenticeshipType);

        // Assert
        var model = addressSearch.As<ViewResult>().Model as AddTrainingVenueViewModel;
        model.SubmitButtonText.Should().Be(expectedSubmitButtonText);
    }

    [Test, MoqAutoData]
    public void LookupAddressAdd_SessionIsNull_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddTrainingVenueController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.LookupAddressAdd(apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void LookupAddressAdd_LocationsAvailableIsTrueInSession_RedirectsToSelectShortCourseTrainingVenue(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddTrainingVenueController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.LocationsAvailable = true;

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.LookupAddressAdd(apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.SelectShortCourseTrainingVenue);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }
}
