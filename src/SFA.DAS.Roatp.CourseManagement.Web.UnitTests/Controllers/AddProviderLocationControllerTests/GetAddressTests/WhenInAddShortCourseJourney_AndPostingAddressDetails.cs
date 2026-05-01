using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderLocationControllerTests.GetAddressTests;

public class WhenInAddShortCourseJourney_AndPostingAddressDetails
{
    private ApprenticeshipType _learningType;

    [SetUp]
    public void Before_Each_Test()
    {
        _learningType = ApprenticeshipType.ApprenticeshipUnit;
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_ReturnsExpectedViewModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressSearchSubmitModel submitModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.GetAddress(submitModel, _learningType) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be(AddProviderLocationController.ViewPath);
        result.Model.Should().BeOfType<AddProviderLocationViewModel>();
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_RouteIsPostAddProviderLocation(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressSearchSubmitModel submitModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.GetAddress(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.Route.Should().Be(RouteNames.PostAddProviderLocation);
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_IsAddJourneyIsTrue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressSearchSubmitModel submitModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.GetAddress(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.IsAddJourney.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_CorrectDisplayHeaderIsReturned(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressSearchSubmitModel submitModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.GetAddress(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.DisplayHeader.Should().Be("Add an apprenticeship unit");
    }

    [Test, MoqAutoData]
    public void When_HasSeenSummaryPageIsTrue_Then_SubmitButtonTextIsConfirm(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressSearchSubmitModel submitModel)
    {
        // Arrange
        sessionModel.HasSeenSummaryPage = true;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.GetAddress(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.SubmitButtonText.Should().Be(ButtonText.Confirm);
    }

    [Test, MoqAutoData]
    public void When_HasSeenSummaryPageIsFalse_Then_SubmitButtonTextIsContinue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressSearchSubmitModel submitModel)
    {
        // Arrange
        sessionModel.HasSeenSummaryPage = false;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.GetAddress(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.SubmitButtonText.Should().Be(ButtonText.Continue);
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsValid_Then_VerifySelectedAddressIsSetInTempData(
        [Frozen] Mock<IValidator<AddressSearchSubmitModel>> validator,
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
        validator.Setup(x => x.Validate(It.IsAny<AddressSearchSubmitModel>())).Returns(new ValidationResult());
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;

        // Act
        sut.GetAddress(submitModel, _learningType);

        // Assert
        tempDataMock.Verify(t => t.Add(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, expectedValueInTempData));
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsValid_Then_RedirectsToGetConfirmAddTrainingVenue(
        [Frozen] Mock<IValidator<AddressSearchSubmitModel>> validator,
        [Greedy] AddProviderLocationController sut,
        AddressSearchSubmitModel submitModel,
        Mock<ITempDataDictionary> tempDataMock)
    {
        // Arrange
        validator.Setup(x => x.Validate(It.IsAny<AddressSearchSubmitModel>())).Returns(new ValidationResult());
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;

        // Act
        var response = sut.GetAddress(submitModel, _learningType);

        // Assert
        var result = response as RedirectToRouteResult;
        result.RouteName.Should().Be(RouteNames.GetConfirmAddProviderLocation);
    }

    [Test, MoqAutoData]
    public void When_SessionIsNull_Then_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddProviderLocationController sut,
        AddressSearchSubmitModel submitMode)
    {
        // Arrange

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.GetAddress(submitMode, _learningType) as RedirectToRouteResult;

        // Assert
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
