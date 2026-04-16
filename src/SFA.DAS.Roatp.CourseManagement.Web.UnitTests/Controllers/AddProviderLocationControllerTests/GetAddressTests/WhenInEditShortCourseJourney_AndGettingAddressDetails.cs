using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderLocationControllerTests.GetAddressTests;
public class WhenInEditShortCourseJourney_AndGettingAddressDetails
{
    private ApprenticeshipType _learningType;

    [SetUp]
    public void Before_Each_Test()
    {
        _learningType = ApprenticeshipType.ApprenticeshipUnit;
    }

    [Test, MoqAutoData]
    public async Task Then_ReturnsExpectedViewModel(
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddProviderLocationController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(() => new GetAllProviderLocationsQueryResult());

        // Act
        var result = await sut.GetAddress(_learningType, larsCode) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be(AddProviderLocationController.ViewPath);
        result.Model.Should().BeOfType<AddProviderLocationViewModel>();
    }

    [Test, MoqAutoData]
    public async Task Then_RouteIsPostAddProviderLocationEditCourse(
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddProviderLocationController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(() => new GetAllProviderLocationsQueryResult());

        // Act
        var result = await sut.GetAddress(_learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.Route.Should().Be(RouteNames.PostAddProviderLocationEditCourse);
    }

    [Test, MoqAutoData]
    public async Task Then_IsAddJourneyIsFalse(
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddProviderLocationController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(() => new GetAllProviderLocationsQueryResult());

        // Act
        var result = await sut.GetAddress(_learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.IsAddJourney.Should().Be(false);
    }

    [Test, MoqAutoData]
    public async Task Then_SubmitButtonTextIsContinue(
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddProviderLocationController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(() => new GetAllProviderLocationsQueryResult());

        // Act
        var result = await sut.GetAddress(_learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.SubmitButtonText.Should().Be(ButtonText.Continue);
    }

    [Test, MoqAutoData]
    public async Task Then_CorrectDisplayHeaderIsReturned(
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddProviderLocationController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(() => new GetAllProviderLocationsQueryResult());

        // Act
        var result = await sut.GetAddress(_learningType, larsCode) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.DisplayHeader.Should().Be("Manage an apprenticeship unit");
    }

    [Test, MoqAutoData]
    public async Task Then_VerifyTempDataIsRemoved(
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddProviderLocationController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(() => new GetAllProviderLocationsQueryResult());

        // Act
        await sut.GetAddress(_learningType, larsCode);

        // Assert
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
    }

    [Test, MoqAutoData]
    public async Task When_ProviderCourseDoesNotExist_Then_RedirectToPageNotFound(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddProviderLocationController sut,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        sut.TempData = tempDataMock.Object;

        // Act
        var result = await sut.GetAddress(_learningType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    [Test, MoqAutoData]
    public async Task When_ProviderLocationsExist_RedirectToEditShortCourseTrainingVenues(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddProviderLocationController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        sut.TempData = tempDataMock.Object;

        // Act
        var result = await sut.GetAddress(_learningType, larsCode) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.EditShortCourseTrainingVenues);
    }
}
