using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddNationalLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseNationalDeliveryControllerTests;

public class EditShortCourseNationalDeliveryControllerPostTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseNationalDelivery_InvalidModelState_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseNationalDeliveryController sut,
        ConfirmNationalDeliverySubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        // Act
        var result = await sut.EditShortCourseNationalDelivery(submitModel, apprenticeshipType, larsCode);

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
    public async Task EditShortCourseNationalDelivery_ProviderCourseDoesNotExist_VerifyMediatorIsInvokedAndRedirectedToEditShortCourseNationalDelivery(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<ConfirmNationalDeliverySubmitModel>> validator,
        [Greedy] EditShortCourseNationalDeliveryController sut,
        ConfirmNationalDeliverySubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        validator.Setup(x => x.Validate(It.IsAny<ConfirmNationalDeliverySubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseNationalDelivery(submitModel, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.EditShortCourseNationalDelivery);
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseNationalDelivery_HasNationalDeliveryOptionChangedToTrue_VerifyMediatorsAreInvokedCorrectly(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<ConfirmNationalDeliverySubmitModel>> validator,
        [Greedy] EditShortCourseNationalDeliveryController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        ConfirmNationalDeliverySubmitModel submitModel,
    string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        providerCourseDetailsApiResponse.ProviderCourseLocations = new List<ProviderCourseLocation>()
        {
            new ProviderCourseLocation()
            {
                SubregionName = "Test",
                LocationType = LocationType.Regional
            }
        };

        submitModel.HasNationalDeliveryOption = true;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        validator.Setup(x => x.Validate(It.IsAny<ConfirmNationalDeliverySubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();


        // Act
        await sut.EditShortCourseNationalDelivery(submitModel, apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<AddNationalLocationToStandardCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseNationalDelivery_HasNationalDeliveryOptionChangedTofFalse_VerifyMediatorsNotInvokedAndRedirectsToEditShortCourseRegions(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<ConfirmNationalDeliverySubmitModel>> validator,
        [Greedy] EditShortCourseNationalDeliveryController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        ConfirmNationalDeliverySubmitModel submitModel,
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

        submitModel.HasNationalDeliveryOption = false;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        validator.Setup(x => x.Validate(It.IsAny<ConfirmNationalDeliverySubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();


        // Act
        var result = await sut.EditShortCourseNationalDelivery(submitModel, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.EditShortCourseRegions);
        mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations), It.IsAny<CancellationToken>()), Times.Never);
        mediatorMock.Verify(m => m.Send(It.Is<AddNationalLocationToStandardCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    [MoqInlineAutoData(LocationType.Regional, false)]
    [MoqInlineAutoData(LocationType.National, true)]
    public async Task EditShortCourseNationalDelivery_NoChangeToHasNationalDeliveryOption_VerifyMediatorsNotInvokedAndRedirectsToManageShortCourseDetails(
        LocationType locationType,
        bool hasNationalDeliveryOption,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<ConfirmNationalDeliverySubmitModel>> validator,
        [Greedy] EditShortCourseNationalDeliveryController sut,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        ConfirmNationalDeliverySubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        providerCourseDetailsApiResponse.ProviderCourseLocations = new List<ProviderCourseLocation>()
        {
            new ProviderCourseLocation()
            {
                SubregionName = "Test",
                LocationType = locationType
            }
        };

        submitModel.HasNationalDeliveryOption = hasNationalDeliveryOption;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        validator.Setup(x => x.Validate(It.IsAny<ConfirmNationalDeliverySubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();


        // Act
        var result = await sut.EditShortCourseNationalDelivery(submitModel, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations), It.IsAny<CancellationToken>()), Times.Never);
        mediatorMock.Verify(m => m.Send(It.Is<AddNationalLocationToStandardCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Never);
    }
}
