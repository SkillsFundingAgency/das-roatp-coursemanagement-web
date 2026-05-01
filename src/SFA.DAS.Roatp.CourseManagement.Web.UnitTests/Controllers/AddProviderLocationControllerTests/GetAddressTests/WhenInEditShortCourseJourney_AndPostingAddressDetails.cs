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
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderLocationControllerTests.GetAddressTests;

public class WhenInEditShortCourseJourney_AndPostingAddressDetails
{
    private ApprenticeshipType _learningType;

    [SetUp]
    public void Before_Each_Test()
    {
        _learningType = ApprenticeshipType.ApprenticeshipUnit;
    }

    [Test, MoqAutoData]
    public void When_ModelIsInvalid_Then_ReturnsExpectedViewModel(
        [Greedy] AddProviderLocationController sut,
        AddressSearchSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        // Act
        var result = sut.GetAddress(submitModel, _learningType, larsCode) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be(AddProviderLocationController.ViewPath);
        result.Model.Should().BeOfType<AddProviderLocationViewModel>();
    }

    [Test, MoqAutoData]
    public void When_ModelIsInvalid_Then_RouteIsPostAddProviderLocationEditCourse(
        [Greedy] AddProviderLocationController sut,
        AddressSearchSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        // Act
        var result = sut.GetAddress(submitModel, _learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.Route.Should().Be(RouteNames.PostAddProviderLocationEditCourse);
    }

    [Test, MoqAutoData]
    public void When_ModelIsInvalid_Then_IsAddJourneyIsFalse(
        [Greedy] AddProviderLocationController sut,
        AddressSearchSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        // Act
        var result = sut.GetAddress(submitModel, _learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.IsAddJourney.Should().Be(false);
    }

    [Test, MoqAutoData]
    public void When_ModelIsInvalid_Then_SubmitButtonTextIsContinue(
        [Greedy] AddProviderLocationController sut,
        AddressSearchSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        // Act
        var result = sut.GetAddress(submitModel, _learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.SubmitButtonText.Should().Be(ButtonText.Continue);
    }

    [Test, MoqAutoData]
    public void When_ModelIsInvalid_Then_CorrectDisplayHeaderIsReturned(
        [Greedy] AddProviderLocationController sut,
        AddressSearchSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        // Act
        var result = sut.GetAddress(submitModel, _learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.DisplayHeader.Should().Be("Manage an apprenticeship unit");
    }

    [Test, MoqAutoData]
    public void When_ModelIsValid_Then_VerifySelectedAddressIsAddedInTempData(
        [Frozen] Mock<IValidator<AddressSearchSubmitModel>> validator,
        [Greedy] AddProviderLocationController sut,
        AddressSearchSubmitModel submitModel,
        Mock<ITempDataDictionary> tempDataMock,
        string larsCode)
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
        sut.GetAddress(submitModel, _learningType, larsCode);

        // Assert
        tempDataMock.Verify(t => t.Add(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, expectedValueInTempData));
    }

    [Test, MoqAutoData]
    public void When_ModelIsValid_Then_RedirectsToGetConfirmAddProviderLocationEditCourse(
    [Frozen] Mock<IValidator<AddressSearchSubmitModel>> validator,
    [Greedy] AddProviderLocationController sut,
    AddressSearchSubmitModel submitModel,
    Mock<ITempDataDictionary> tempDataMock,
    string larsCode)
    {
        // Arrange
        validator.Setup(x => x.Validate(It.IsAny<AddressSearchSubmitModel>())).Returns(new ValidationResult());
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;

        // Act
        var response = sut.GetAddress(submitModel, _learningType, larsCode);

        // Assert
        var result = response as RedirectToRouteResult;
        result.RouteName.Should().Be(RouteNames.GetConfirmAddProviderLocationEditCourse);
    }
}
