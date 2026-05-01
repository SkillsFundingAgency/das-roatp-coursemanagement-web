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
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateOnlineDeliveryOption;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseLocationOptionsControllerTests;

public class EditShortCourseLocationOptionsControllerPostTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_InvalidModelState_ReturnsView(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseLocationOptionsController sut,
    SelectShortCourseLocationOptionsSubmitModel submitModel,
    string larsCode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        List<ShortCourseLocationOptionModel> locationOptions = new()
        {
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.ProviderLocation, IsSelected = false },
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.EmployerLocation, IsSelected = false },
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.Online, IsSelected = false },
        };

        var providerCourseDetailsApiResponse = new GetProviderCourseDetailsQueryResult();

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);


        // Act
        var result = await sut.EditShortCourseLocationOptions(submitModel, apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult.Model as SelectShortCourseLocationOptionsViewModel;
        model.LocationOptions.Should().BeEquivalentTo(locationOptions);
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        model.SubmitButtonText.Should().Be(ButtonText.Confirm);
        model.Route.Should().Be(RouteNames.EditShortCourseLocationOptions);
        model.IsAddJourney.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_ProviderCourseDoesNotExist_VerifyMediatorIsInvokedAndRedirectedToEditShortCourseLocationOptions(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<SelectShortCourseLocationOptionsSubmitModel>> validator,
    [Greedy] EditShortCourseLocationOptionsController sut,
    SelectShortCourseLocationOptionsSubmitModel submitModel,
    string larsCode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        validator.Setup(x => x.Validate(It.IsAny<SelectShortCourseLocationOptionsSubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);


        // Act
        var result = await sut.EditShortCourseLocationOptions(submitModel, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.EditShortCourseLocationOptions);
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_OnlineLocationIsAdded_VerifyUpdateOnlineDeliveryMediatorIsInvokedWithTrueValue(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<SelectShortCourseLocationOptionsSubmitModel>> validator,
    [Greedy] EditShortCourseLocationOptionsController sut,
    SelectShortCourseLocationOptionsSubmitModel submitModel,
    string larsCode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        submitModel.SelectedLocationOptions = new List<ShortCourseLocationOption>();
        submitModel.SelectedLocationOptions.Add(ShortCourseLocationOption.Online);

        var providerCourseDetailsApiResponse = new GetProviderCourseDetailsQueryResult()
        {
            HasOnlineDeliveryOption = false,
            ProviderCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation { LocationType = LocationType.Provider },
                new ProviderCourseLocation { LocationType = LocationType.National },
                new ProviderCourseLocation { LocationType = LocationType.Regional }
            }
        };

        validator.Setup(x => x.Validate(It.IsAny<SelectShortCourseLocationOptionsSubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        // Act
        await sut.EditShortCourseLocationOptions(submitModel, apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<UpdateOnlineDeliveryOptionCommand>(c =>
        c.Ukprn == int.Parse(TestConstants.DefaultUkprn) &&
        c.UserId == TestConstants.DefaultUserId &&
        c.LarsCode == larsCode &&
        c.HasOnlineDeliveryOption), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_OnlineLocationIsRemoved_VerifyUpdateOnlineDeliveryMediatorIsInvokedWithFalseValue(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<SelectShortCourseLocationOptionsSubmitModel>> validator,
    [Greedy] EditShortCourseLocationOptionsController sut,
    SelectShortCourseLocationOptionsSubmitModel submitModel,
    string larsCode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        submitModel.SelectedLocationOptions = new List<ShortCourseLocationOption>();
        submitModel.SelectedLocationOptions.Add(ShortCourseLocationOption.ProviderLocation);

        var providerCourseDetailsApiResponse = new GetProviderCourseDetailsQueryResult()
        {
            HasOnlineDeliveryOption = true,
            ProviderCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation { LocationType = LocationType.Provider },
                new ProviderCourseLocation { LocationType = LocationType.National },
                new ProviderCourseLocation { LocationType = LocationType.Regional }
            }
        };

        validator.Setup(x => x.Validate(It.IsAny<SelectShortCourseLocationOptionsSubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        // Act
        await sut.EditShortCourseLocationOptions(submitModel, apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<UpdateOnlineDeliveryOptionCommand>(c =>
        c.Ukprn == int.Parse(TestConstants.DefaultUkprn) &&
        c.UserId == TestConstants.DefaultUserId &&
        c.LarsCode == larsCode &&
        !c.HasOnlineDeliveryOption), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_ProviderLocationIsAdded_DeleteCommandIsNotInvokedAndRedirectsToEditShortCourseTrainingVenues(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<SelectShortCourseLocationOptionsSubmitModel>> validator,
        [Greedy] EditShortCourseLocationOptionsController sut,
        SelectShortCourseLocationOptionsSubmitModel submitModel,
        string larsCode
        )
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        submitModel.SelectedLocationOptions = new List<ShortCourseLocationOption>();
        submitModel.SelectedLocationOptions.Add(ShortCourseLocationOption.ProviderLocation);

        var providerCourseDetailsApiResponse = new GetProviderCourseDetailsQueryResult()
        {
            HasOnlineDeliveryOption = false,
            ProviderCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation { LocationType = LocationType.National },
                new ProviderCourseLocation { LocationType = LocationType.Regional }
            }
        };

        validator.Setup(x => x.Validate(It.IsAny<SelectShortCourseLocationOptionsSubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        // Act
        var result = await sut.EditShortCourseLocationOptions(submitModel, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.EditShortCourseTrainingVenues);
        mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(q =>
       q.Ukprn.ToString() == TestConstants.DefaultUkprn &&
       q.LarsCode == larsCode &&
       q.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteProviderLocations),
       It.IsAny<CancellationToken>()), Times.Never());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_ProviderLocationIsRemoved_DeleteCommandIsInvokedAndRedirectsToManageShortCourseDetails(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<SelectShortCourseLocationOptionsSubmitModel>> validator,
    [Greedy] EditShortCourseLocationOptionsController sut,
    SelectShortCourseLocationOptionsSubmitModel submitModel,
    string larsCode
    )
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        submitModel.SelectedLocationOptions = new List<ShortCourseLocationOption>();

        var providerCourseDetailsApiResponse = new GetProviderCourseDetailsQueryResult()
        {
            HasOnlineDeliveryOption = false,
            ProviderCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation { LocationType = LocationType.Provider }
            }
        };

        validator.Setup(x => x.Validate(It.IsAny<SelectShortCourseLocationOptionsSubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        // Act
        var result = await sut.EditShortCourseLocationOptions(submitModel, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(q =>
        q.Ukprn.ToString() == TestConstants.DefaultUkprn &&
        q.LarsCode == larsCode &&
        q.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteProviderLocations),
        It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_EmployerLocationIsAdded_SessionIsSet(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IValidator<SelectShortCourseLocationOptionsSubmitModel>> validator,
    [Greedy] EditShortCourseLocationOptionsController sut,
    SelectShortCourseLocationOptionsSubmitModel submitModel,
    string larsCode
    )
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        submitModel.SelectedLocationOptions = new List<ShortCourseLocationOption>();
        submitModel.SelectedLocationOptions.Add(ShortCourseLocationOption.EmployerLocation);

        var providerCourseDetailsApiResponse = new GetProviderCourseDetailsQueryResult()
        {
            HasOnlineDeliveryOption = false,
            ProviderCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation { LocationType = LocationType.Provider }
            }
        };

        validator.Setup(x => x.Validate(It.IsAny<SelectShortCourseLocationOptionsSubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        // Act
        await sut.EditShortCourseLocationOptions(submitModel, apprenticeshipType, larsCode);

        // Assert
        sessionServiceMock.Verify(s => s.Set(ShortCourseLocationOption.EmployerLocation.ToString(), SessionKeys.SelectedShortCourseLocationOption), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_EmployerLocationIsAdded_DeleteCommandIsNotInvokedAndRedirectsToEditShortCourseNationalDelivery(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<SelectShortCourseLocationOptionsSubmitModel>> validator,
    [Greedy] EditShortCourseLocationOptionsController sut,
    SelectShortCourseLocationOptionsSubmitModel submitModel,
    string larsCode
    )
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        submitModel.SelectedLocationOptions = new List<ShortCourseLocationOption>();
        submitModel.SelectedLocationOptions.Add(ShortCourseLocationOption.EmployerLocation);

        var providerCourseDetailsApiResponse = new GetProviderCourseDetailsQueryResult()
        {
            HasOnlineDeliveryOption = false,
            ProviderCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation { LocationType = LocationType.Provider }
            }
        };

        validator.Setup(x => x.Validate(It.IsAny<SelectShortCourseLocationOptionsSubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        // Act
        var result = await sut.EditShortCourseLocationOptions(submitModel, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.EditShortCourseNationalDelivery);
        mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(q =>
        q.Ukprn.ToString() == TestConstants.DefaultUkprn &&
        q.LarsCode == larsCode &&
        q.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations),
        It.IsAny<CancellationToken>()), Times.Never());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_EmployerLocationIsRemoved_DeleteCommandIsInvokedAndRedirectsToManageShortCourseDetails(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<SelectShortCourseLocationOptionsSubmitModel>> validator,
    [Greedy] EditShortCourseLocationOptionsController sut,
    SelectShortCourseLocationOptionsSubmitModel submitModel,
    string larsCode
    )
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        submitModel.SelectedLocationOptions = new List<ShortCourseLocationOption>();

        var providerCourseDetailsApiResponse = new GetProviderCourseDetailsQueryResult()
        {
            HasOnlineDeliveryOption = false,
            ProviderCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation { LocationType = LocationType.National }
            }
        };

        validator.Setup(x => x.Validate(It.IsAny<SelectShortCourseLocationOptionsSubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        // Act
        var result = await sut.EditShortCourseLocationOptions(submitModel, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(q =>
        q.Ukprn.ToString() == TestConstants.DefaultUkprn &&
        q.LarsCode == larsCode &&
        q.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations),
        It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_NoChange_MediatorsAreNotInvokedAndRedirectsToManageShortCourseDetails(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<SelectShortCourseLocationOptionsSubmitModel>> validator,
    [Greedy] EditShortCourseLocationOptionsController sut,
    SelectShortCourseLocationOptionsSubmitModel submitModel,
    string larsCode
    )
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        submitModel.SelectedLocationOptions = new List<ShortCourseLocationOption>();
        submitModel.SelectedLocationOptions.Add(ShortCourseLocationOption.Online);
        submitModel.SelectedLocationOptions.Add(ShortCourseLocationOption.ProviderLocation);
        submitModel.SelectedLocationOptions.Add(ShortCourseLocationOption.EmployerLocation);

        var providerCourseDetailsApiResponse = new GetProviderCourseDetailsQueryResult()
        {
            HasOnlineDeliveryOption = true,
            ProviderCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation { LocationType = LocationType.Provider },
                new ProviderCourseLocation { LocationType = LocationType.National },
                new ProviderCourseLocation { LocationType = LocationType.Regional }
            }
        };

        validator.Setup(x => x.Validate(It.IsAny<SelectShortCourseLocationOptionsSubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        // Act
        var result = await sut.EditShortCourseLocationOptions(submitModel, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        mediatorMock.Verify(m => m.Send(It.IsAny<UpdateOnlineDeliveryOptionCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        mediatorMock.Verify(m => m.Send(It.IsAny<DeleteCourseLocationsCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    }
}
