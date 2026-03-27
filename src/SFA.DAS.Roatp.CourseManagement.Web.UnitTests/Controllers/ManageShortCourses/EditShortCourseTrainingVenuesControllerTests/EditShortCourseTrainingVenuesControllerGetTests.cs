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
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseTrainingVenuesControllerTests;
public class EditShortCourseTrainingVenuesControllerGetTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseTrainingVenues_ProviderCourseExists_ReturnsView(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseTrainingVenuesController sut,
    GetStandardDetailsQueryResult provideCourseDetailsApiResponse,
    GetAllProviderLocationsQueryResult providerLocationsApiResponse,
    string larscode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        List<TrainingVenueModel> trainingVenues = providerLocationsApiResponse.ProviderLocations.Select(p => (TrainingVenueModel)p).Where(p => p.LocationType == LocationType.Provider).OrderBy(l => l.LocationName).ToList();

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larscode), It.IsAny<CancellationToken>())).ReturnsAsync(provideCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        // Act
        var result = await sut.EditShortCourseTrainingVenues(apprenticeshipType, larscode);

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
    public async Task EditShortCourseTrainingVenues_ProviderCourseExists_IsSelectedIsSetCorrectly(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseTrainingVenuesController sut,
    GetStandardDetailsQueryResult provideCourseDetailsApiResponse,
    GetAllProviderLocationsQueryResult providerLocationsApiResponse,
    string larscode
)
    {
        List<ProviderCourseLocation> providerCourseLocations = new List<ProviderCourseLocation>()
        {
            new ProviderCourseLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider
            }
        };

        List<ProviderLocation> providerLocations = new List<ProviderLocation>()
        {
            new ProviderLocation
            {
                LocationName = "Test",
                LocationType = LocationType.Provider
            }
        };

        provideCourseDetailsApiResponse.ProviderCourseLocations = providerCourseLocations;

        providerLocationsApiResponse.ProviderLocations = providerLocations;

        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larscode), It.IsAny<CancellationToken>())).ReturnsAsync(provideCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        // Act
        var result = await sut.EditShortCourseTrainingVenues(apprenticeshipType, larscode);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult.Model as ShortCourseTrainingVenuesViewModel;
        model.TrainingVenues.FirstOrDefault().IsSelected.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseTrainingVenues_ProviderCourseExists_VerifyMediatorInvokedCorrectly(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseTrainingVenuesController sut,
    GetStandardDetailsQueryResult provideCourseDetailsApiResponse,
    GetAllProviderLocationsQueryResult providerLocationsApiResponse,
    string larscode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larscode), It.IsAny<CancellationToken>())).ReturnsAsync(provideCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        // Act
        await sut.EditShortCourseTrainingVenues(apprenticeshipType, larscode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larscode), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseTrainingVenues_ProviderCourseDoesNotExist_RedirectToPageNotFound(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseTrainingVenuesController sut,
    string larscode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larscode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        // Act
        var result = await sut.EditShortCourseTrainingVenues(apprenticeshipType, larscode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseTrainingVenues_ProviderLocationsReturnsEmpty_RedirectsToGetAddTrainingVenue(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseTrainingVenuesController sut,
    GetStandardDetailsQueryResult provideCourseDetailsApiResponse,
    string larscode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larscode), It.IsAny<CancellationToken>())).ReturnsAsync(provideCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(new GetAllProviderLocationsQueryResult());

        // Act
        var result = await sut.EditShortCourseTrainingVenues(apprenticeshipType, larscode);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.GetAddTrainingVenue);
    }
}
