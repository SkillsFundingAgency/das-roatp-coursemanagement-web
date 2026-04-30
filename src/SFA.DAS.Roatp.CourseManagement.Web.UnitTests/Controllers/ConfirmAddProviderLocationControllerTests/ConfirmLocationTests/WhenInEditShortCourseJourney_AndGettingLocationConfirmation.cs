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
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmAddProviderLocationControllerTests.ConfirmLocationTests;
public class WhenInEditShortCourseJourney_AndGettingLocationConfirmation
{
    private ApprenticeshipType _learningType;

    [SetUp]
    public void Before_Each_Test()
    {
        _learningType = ApprenticeshipType.ApprenticeshipUnit;
    }

    [Test, MoqAutoData]
    public void When_AddressIsInTempData_Then_ReturnsExpectedViewModel(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        // Act
        var result = sut.ConfirmLocation(_learningType, larsCode) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        result.ViewName.Should().Be(ConfirmAddProviderLocationController.ViewPath);
        result.Model.Should().BeOfType<ConfirmAddProviderLocationViewModel>();
    }

    [Test, MoqAutoData]
    public void When_AddressIsInTempData_Then_ReturnExpectedAddress(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        // Act
        var result = sut.ConfirmLocation(_learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.AddressLine1.Should().Be(addressItem.AddressLine1);
        model!.AddressLine2.Should().Be(addressItem.AddressLine2);
        model!.Town.Should().Be(addressItem.Town);
        model!.Postcode.Should().Be(addressItem.Postcode);
    }

    [Test, MoqAutoData]
    public void When_AddressIsInTempData_Then_RouteIsPostConfirmAddProviderLocationEditCourse(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        // Act
        var result = sut.ConfirmLocation(_learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.Route.Should().Be(RouteNames.PostConfirmAddProviderLocationEditCourse);
    }

    [Test, MoqAutoData]
    public void When_AddressIsInTempData_Then_ShowCancelButtonIsFalse(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        // Act
        var result = sut.ConfirmLocation(_learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.ShowCancelOption.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public void When_AddressIsInTempData_Then_IsAddJourneyIsFalse(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        // Act
        var result = sut.ConfirmLocation(_learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.IsAddJourney.Should().Be(false);
    }

    [Test, MoqAutoData]
    public void When_AddressIsInTempData_Then_SubmitButtonTextIsConfirm(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        // Act
        var result = sut.ConfirmLocation(_learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.SubmitButtonText.Should().Be(ButtonText.Confirm);
    }

    [Test, MoqAutoData]
    public void When_AddressIsInTempData_Then_CorrectDisplayHeaderIsReturned(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        // Act
        var result = sut.ConfirmLocation(_learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.DisplayHeader.Should().Be("Manage an apprenticeship unit");
    }

    [Test, MoqAutoData]
    public void When_AddressIsInTempData_Then_ReturnsExpectedCancelLinkUrl(
        Mock<ITempDataDictionary> tempDataMock,
        Mock<IUrlHelper> urlHelperMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        string cancelLinkUrl,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddProviderLocation, cancelLinkUrl);

        // Act
        var result = sut.ConfirmLocation(_learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.CancelLink.Should().Be(cancelLinkUrl);
    }

    [Test, MoqAutoData]
    public void When_AddressNotInTempData_Then_RedirectsToEditShortCourseTrainingVenues(
        string larsCode,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut)
    {
        // Arrange
        object address = null;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocation(_learningType, larsCode) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.EditShortCourseTrainingVenues);
    }
}
