using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseTrainingVenuesControllerTests;
public class EditShortCourseTrainingVenuesControllerPostTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseTrainingVenues_InvalidModelState_ReturnsView(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseTrainingVenuesController sut,
    GetAllProviderLocationsQueryResult providerLocationsApiResponse,
    ShortCourseTrainingVenuesSubmitModel submitModel,
    string larscode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        List<TrainingVenueModel> trainingVenues = providerLocationsApiResponse.ProviderLocations.Select(p => (TrainingVenueModel)p).Where(p => p.LocationType == LocationType.Provider).OrderBy(l => l.LocationName).ToList();

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        // Act
        var result = await sut.EditShortCourseTrainingVenues(submitModel, apprenticeshipType, larscode);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult.Model as ShortCourseTrainingVenuesViewModel;
        model.TrainingVenues.Should().BeEquivalentTo(trainingVenues);
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        model.SubmitButtonText.Should().Be(ButtonText.Confirm);
        model.Route.Should().Be(RouteNames.EditShortCourseTrainingVenues);
        model.IsAddJourney.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseTrainingVenues_ProviderLocationsReturnsEmpty_VerifyMediatorInvokedCorrectlyAndRedirectsToEditShortCourseTrainingVenues(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseTrainingVenuesController sut,
    ShortCourseTrainingVenuesSubmitModel submitModel,
    string larscode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(new GetAllProviderLocationsQueryResult());

        // Act
        var result = await sut.EditShortCourseTrainingVenues(submitModel, apprenticeshipType, larscode);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.EditShortCourseTrainingVenues);
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseTrainingVenues_TrainingVenueAddedOnly_SendsAddCommandAndVerifyMediatorIsInvoked(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseTrainingVenuesController sut,
        GetStandardDetailsQueryResult providerCourseDetailsApiResponse,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        var providerCourseLocationId = Guid.NewGuid();
        var providerLocationsId = Guid.NewGuid();
        var addProviderLocationsId = Guid.NewGuid();

        List<ProviderCourseLocation> providerCourseLocations = new List<ProviderCourseLocation>()
        {
            new ProviderCourseLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider,
                Id = providerCourseLocationId,
            }
        };

        List<ProviderLocation> providerLocations = new List<ProviderLocation>()
        {
            new ProviderLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider,
                NavigationId = providerLocationsId,
            },
            new ProviderLocation
            {
                LocationName = "Test2",
                LocationType = LocationType.Provider,
                NavigationId = addProviderLocationsId,
            }
        };

        var submitModel = new ShortCourseTrainingVenuesSubmitModel();

        submitModel.SelectedProviderLocationIds.Add(providerLocationsId);
        submitModel.SelectedProviderLocationIds.Add(addProviderLocationsId);

        providerCourseDetailsApiResponse.ProviderCourseLocations = providerCourseLocations;

        providerLocationsApiResponse.ProviderLocations = providerLocations;

        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        // Act
        await sut.EditShortCourseTrainingVenues(submitModel, apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<AddProviderCourseLocationCommand>(c =>
        c.Ukprn == int.Parse(TestConstants.DefaultUkprn) &&
        c.UserId == TestConstants.DefaultUserId &&
        c.LarsCode == larsCode &&
        c.LocationNavigationId == addProviderLocationsId &&
        c.HasDayReleaseDeliveryOption == false &&
        c.HasBlockReleaseDeliveryOption == false), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.IsAny<DeleteProviderCourseLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseTrainingVenues_TrainingVenueRemovedOnly_SendsDeleteCommandAndVerifyMediatorIsInvoked(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseTrainingVenuesController sut,
        GetStandardDetailsQueryResult providerCourseDetailsApiResponse,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        var providerCourseLocationId = Guid.NewGuid();
        var deleteProviderCourseLocationId = Guid.NewGuid();
        var providerLocationsId = Guid.NewGuid();
        var addProviderLocationsId = Guid.NewGuid();

        List<ProviderCourseLocation> providerCourseLocations = new List<ProviderCourseLocation>()
        {
            new ProviderCourseLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider,
                Id = providerCourseLocationId,
            },
            new ProviderCourseLocation
            {
                LocationName = "Test2",
                LocationType = LocationType.Provider,
                Id = deleteProviderCourseLocationId,
            }
        };

        List<ProviderLocation> providerLocations = new List<ProviderLocation>()
        {
            new ProviderLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider,
                NavigationId = providerLocationsId,
            },
            new ProviderLocation
            {
                LocationName = "Test2",
                LocationType = LocationType.Provider,
                NavigationId = addProviderLocationsId,
            }
        };

        var submitModel = new ShortCourseTrainingVenuesSubmitModel();

        submitModel.SelectedProviderLocationIds.Add(providerLocationsId);

        providerCourseDetailsApiResponse.ProviderCourseLocations = providerCourseLocations;

        providerLocationsApiResponse.ProviderLocations = providerLocations;

        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        // Act
        await sut.EditShortCourseTrainingVenues(submitModel, apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.IsAny<AddProviderCourseLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        mediatorMock.Verify(m => m.Send(It.Is<DeleteProviderCourseLocationCommand>(c =>
        c.Ukprn == int.Parse(TestConstants.DefaultUkprn) &&
        c.UserId == TestConstants.DefaultUserId &&
        c.LarsCode == larsCode &&
        c.Id == deleteProviderCourseLocationId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseTrainingVenues_TrainingVenueAddedAndRemoved_SendsAddAndDeleteCommandAndVerifyMediatorIsInvoked(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseTrainingVenuesController sut,
        GetStandardDetailsQueryResult providerCourseDetailsApiResponse,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        var deleteProviderCourseLocationId = Guid.NewGuid();
        var providerLocationsId = Guid.NewGuid();
        var addProviderLocationsId = Guid.NewGuid();

        List<ProviderCourseLocation> providerCourseLocations = new List<ProviderCourseLocation>()
        {
            new ProviderCourseLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider,
                Id = deleteProviderCourseLocationId,
            }
        };

        List<ProviderLocation> providerLocations = new List<ProviderLocation>()
        {
            new ProviderLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider,
                NavigationId = providerLocationsId,
            },
            new ProviderLocation
            {
                LocationName = "Test2",
                LocationType = LocationType.Provider,
                NavigationId = addProviderLocationsId,
            }
        };

        var submitModel = new ShortCourseTrainingVenuesSubmitModel();

        submitModel.SelectedProviderLocationIds.Add(addProviderLocationsId);

        providerCourseDetailsApiResponse.ProviderCourseLocations = providerCourseLocations;

        providerLocationsApiResponse.ProviderLocations = providerLocations;

        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        // Act
        await sut.EditShortCourseTrainingVenues(submitModel, apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<AddProviderCourseLocationCommand>(c =>
        c.Ukprn == int.Parse(TestConstants.DefaultUkprn) &&
        c.UserId == TestConstants.DefaultUserId &&
        c.LarsCode == larsCode &&
        c.LocationNavigationId == addProviderLocationsId &&
        c.HasDayReleaseDeliveryOption == false &&
        c.HasBlockReleaseDeliveryOption == false), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<DeleteProviderCourseLocationCommand>(c =>
        c.Ukprn == int.Parse(TestConstants.DefaultUkprn) &&
        c.UserId == TestConstants.DefaultUserId &&
        c.LarsCode == larsCode &&
        c.Id == deleteProviderCourseLocationId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseTrainingVenues_NoChange_VerifyMediatorIsNotInvokedAndRedirectsToManageShortCourseDetails(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseTrainingVenuesController sut,
        GetStandardDetailsQueryResult providerCourseDetailsApiResponse,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        var providerCourseLocationId = Guid.NewGuid();
        var providerLocationsId = Guid.NewGuid();

        List<ProviderCourseLocation> providerCourseLocations = new List<ProviderCourseLocation>()
        {
            new ProviderCourseLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider,
                Id = providerCourseLocationId,
            }
        };

        List<ProviderLocation> providerLocations = new List<ProviderLocation>()
        {
            new ProviderLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider,
                NavigationId = providerLocationsId,
            },
        };

        var submitModel = new ShortCourseTrainingVenuesSubmitModel();

        submitModel.SelectedProviderLocationIds.Add(providerLocationsId);

        providerCourseDetailsApiResponse.ProviderCourseLocations = providerCourseLocations;

        providerLocationsApiResponse.ProviderLocations = providerLocations;

        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        // Act
        var result = await sut.EditShortCourseTrainingVenues(submitModel, apprenticeshipType, larsCode);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        mediatorMock.Verify(m => m.Send(It.IsAny<AddProviderCourseLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        mediatorMock.Verify(m => m.Send(It.IsAny<DeleteProviderCourseLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseTrainingVenues_NonProviderLocationTypes_VerifyMediatorIsNotInvokedForNonProvider(
       [Frozen] Mock<IMediator> mediatorMock,
       [Greedy] EditShortCourseTrainingVenuesController sut,
       GetStandardDetailsQueryResult providerCourseDetailsApiResponse,
       GetAllProviderLocationsQueryResult providerLocationsApiResponse,
       string larsCode)
    {
        var providerCourseLocationId = Guid.NewGuid();
        var providerLocationsId = Guid.NewGuid();
        var nonProviderCourseLocationsId = Guid.NewGuid();

        List<ProviderCourseLocation> providerCourseLocations = new List<ProviderCourseLocation>()
        {
            new ProviderCourseLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider,
                Id = providerCourseLocationId,
            },
            new ProviderCourseLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Regional,
                Id = nonProviderCourseLocationsId,
            }
        };

        List<ProviderLocation> providerLocations = new List<ProviderLocation>()
        {
            new ProviderLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider,
                NavigationId = providerLocationsId,
            },
        };

        var submitModel = new ShortCourseTrainingVenuesSubmitModel();

        submitModel.SelectedProviderLocationIds.Add(providerLocationsId);

        providerCourseDetailsApiResponse.ProviderCourseLocations = providerCourseLocations;

        providerLocationsApiResponse.ProviderLocations = providerLocations;

        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        // Act
        await sut.EditShortCourseTrainingVenues(submitModel, apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<DeleteProviderCourseLocationCommand>(c => c.Id == nonProviderCourseLocationsId), It.IsAny<CancellationToken>()), Times.Never);
    }
}
