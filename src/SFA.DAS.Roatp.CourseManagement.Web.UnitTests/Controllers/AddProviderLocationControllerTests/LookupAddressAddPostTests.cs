using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderLocationControllerTests;
public class LookupAddressAddPostTests
{
    [Test]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit)]
    public void When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndModelStateIsInvalid_Then_ReturnsViewResult(
       ApprenticeshipType apprenticeshipType,
       [Frozen] Mock<ISessionService> sessionServiceMock,
       [Greedy] AddProviderLocationController sut,
       AddressSearchSubmitModel model,
       ShortCourseSessionModel shortCourseSessionModel,
       StandardSessionModel standardSessionModel)
    {
        // Arrange

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        // Act
        var result = sut.LookupAddressAdd(model, apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        viewResult.ViewName.Should().Be(AddProviderLocationController.ViewPath);
        var viewModel = viewResult.Model as AddProviderLocationViewModel;
        viewModel.Route.Should().Be(RouteNames.PostAddProviderLocation);
        viewModel.IsAddJourney.Should().Be(true);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship, "Add a standard")]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, "Add an apprenticeship unit")]
    public void When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndModelStateIsInvalid_Then_ReturnsExpectedDisplayHeader(
        ApprenticeshipType apprenticeshipType,
        string expectedDisplayHeader,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        AddressSearchSubmitModel model,
        ShortCourseSessionModel shortCourseSessionModel,
        StandardSessionModel standardSessionModel)
    {
        // Arrange

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        sut.TempData = tempDataMock.Object;

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        // Act
        var addressSearch = sut.LookupAddressAdd(model, apprenticeshipType) as ViewResult;

        // Assert
        var viewModel = addressSearch.Model as AddProviderLocationViewModel;
        viewModel.DisplayHeader.Should().Be(expectedDisplayHeader);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, true, ButtonText.Confirm)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, false, ButtonText.Continue)]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship, false, ButtonText.Continue)]
    public void When_ApprenticeshipTypeIsApprenticeshipUnitAndHasSeenSummaryPageIsTrueOrFalse_Then_ReturnsExpectedButtonText(
       ApprenticeshipType apprenticeshipType,
       bool hasSeenSummaryPage,
       string expectedSubmitButtonText,
       [Frozen] Mock<ISessionService> sessionServiceMock,
       [Greedy] AddProviderLocationController sut,
       AddressSearchSubmitModel model,
       ShortCourseSessionModel shortCourseSessionModel,
       StandardSessionModel standardSessionModel)
    {
        // Arrange
        shortCourseSessionModel.HasSeenSummaryPage = hasSeenSummaryPage;

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        // Act
        var result = sut.LookupAddressAdd(model, apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        var viewModel = viewResult.Model as AddProviderLocationViewModel;
        viewModel.SubmitButtonText.Should().Be(expectedSubmitButtonText);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit)]
    public void When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndModelStateIsValid_Then_SetsSelectedAddressInTempDataAndRedirectsToGetConfirmAddTrainingVenue(
        ApprenticeshipType apprenticeshipType,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddProviderLocationController sut,
        AddressSearchSubmitModel submitModel,
        Mock<ITempDataDictionary> tempDataMock)
    {
        // Arrange

        AddressItem selectedAddress = new AddressItem
        {
            AddressLine1 = submitModel.AddressLine1,
            AddressLine2 = submitModel.AddressLine2,
            Town = submitModel.Town,
            County = submitModel.County,
            Latitude = submitModel.Latitude,
            Longitude = submitModel.Longitude,
            Postcode = submitModel.Postcode,
            Uprn = null
        };

        var expectedValueInTempData = JsonSerializer.Serialize(selectedAddress);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;

        // Act
        var response = sut.LookupAddressAdd(submitModel, apprenticeshipType);

        // Assert
        var result = response as RedirectToRouteResult;
        Assert.IsNotNull(result);
        result.RouteName.Should().Be(RouteNames.GetConfirmAddProviderLocation);
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
        tempDataMock.Verify(t => t.Add(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, expectedValueInTempData));
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit)]
    public void When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndSessionIsNull_Then_RedirectsToReviewYourDetails(
        ApprenticeshipType apprenticeshipType,
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddProviderLocationController sut,
        AddressSearchSubmitModel submitMode)
    {
        // Arrange

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);
        }

        // Act
        var result = sut.LookupAddressAdd(submitMode, apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
