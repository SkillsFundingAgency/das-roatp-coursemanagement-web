using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseNationalDeliveryControllerTests;
public class EditShortCourseNationalDeliveryControllerGetTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseNationalDelivery_ProviderCourseExists_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseNationalDeliveryController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseNationalDelivery(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult.Model as ConfirmNationalDeliveryViewModel;
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        model.SubmitButtonText.Should().Be(ButtonText.Confirm);
        model.Route.Should().Be(RouteNames.EditShortCourseNationalDelivery);
        model.IsAddJourney.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseNationalDelivery_ProviderCourseExists_HasNationalDeliveryOptionIsSetCorrectly(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseNationalDeliveryController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        providerCourseDetailsApiResponse.ProviderCourseLocations = new List<ProviderCourseLocation>()
        {
            new ProviderCourseLocation()
            {
                SubregionName = "Test",
                LocationType = LocationType.National
            }
        };

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseNationalDelivery(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult.Model as ConfirmNationalDeliveryViewModel;
        model.HasNationalDeliveryOption.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseNationalDelivery_ProviderCourseExists_VerifyMediatorIsInvoked(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseNationalDeliveryController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        sut.AddDefaultContextWithUser();

        // Act
        await sut.EditShortCourseNationalDelivery(apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseNationalDelivery_ProviderCourseDoesNotExist_RedirectsToPageNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseNationalDeliveryController sut,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseNationalDelivery(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }
}
