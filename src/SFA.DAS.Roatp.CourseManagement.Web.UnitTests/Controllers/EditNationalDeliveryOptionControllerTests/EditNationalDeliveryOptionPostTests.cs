using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddNationalLocation;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditNationalDeliveryOptionControllerTests;

[TestFixture]
public class EditNationalDeliveryOptionPostTests : EditNationalDeliveryOptionControllerTestBase
{
    [Test, AutoData]
    public async Task Post_SessionModelNotSet_ReturnsNotFound(ConfirmNationalProviderSubmitModel model, string larsCode)
    {
        SetupController();

        var result = await Sut.Index(larsCode, model);

        result.Should().BeOfType<NotFoundResult>();
    }


    [Test, AutoData]
    public async Task Post_InvalidState_ReturnsView(ConfirmNationalProviderSubmitModel model, string larsCode)
    {
        SetupController();
        SetLocationOptionInSession(LocationOption.EmployerLocation);

        var validationResult = new ValidationResult();

        validationResult.Errors.Add(new ValidationFailure("Field", "Error"));

        _validatorMock.Setup(x => x.Validate(It.IsAny<ConfirmNationalProviderSubmitModel>())).Returns(validationResult);

        var result = await Sut.Index(larsCode, model);

        var actual = (ViewResult)result;

        Assert.NotNull(actual);
    }

    [Test, AutoData]
    public async Task Post_DoesNotDeliverNationally_RedirectsToSelectRegions(string larsCode)
    {
        SetupController();
        SetLocationOptionInSession(LocationOption.EmployerLocation);

        ConfirmNationalProviderSubmitModel model = new()
        {
            HasNationalDeliveryOption = false
        };

        var result = await Sut.Index(larsCode, model);

        result.Should().BeOfType<RedirectToRouteResult>().Which.RouteName.Should().Be(RouteNames.GetStandardSubRegions);
        MediatorMock.Verify(m => m.Send(It.IsAny<AddNationalLocationToStandardCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestCase(LocationType.Regional)]
    [TestCase(LocationType.National)]
    public async Task WhenAddingNationalLocation_AndEmployerLocationExists_ThenDeletesEmployerLocation(LocationType locationType)
    {
        SetupController();
        SetLocationOptionInSession(LocationOption.EmployerLocation);
        SetProviderCourseLocationsInMediatorResponse(
        [
            new ProviderCourseLocation { LocationType = locationType }
        ]);
        ConfirmNationalProviderSubmitModel model = new()
        {
            HasNationalDeliveryOption = true
        };

        await Sut.Index("any_lars_Code", model);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations), It.IsAny<CancellationToken>()));
    }

    [Test, AutoData]
    public async Task WhenAddingNationalLocation_ThenInsertsNationalLocation(string larsCode, LocationType locationType)
    {
        SetupController();
        SetLocationOptionInSession(LocationOption.EmployerLocation);
        SetProviderCourseLocationsInMediatorResponse(
        [
            new ProviderCourseLocation { LocationType = locationType }
        ]);
        ConfirmNationalProviderSubmitModel model = new()
        {
            HasNationalDeliveryOption = true
        };

        await Sut.Index(larsCode, model);

        MediatorMock.Verify(m => m.Send(It.IsAny<AddNationalLocationToStandardCommand>(), It.IsAny<CancellationToken>()));
    }

    [Test, AutoData]
    public async Task WhenAddingNationLocation_AndSwitchingToEmployerOption_AndHasProviderLocations_ThenDeletesProviderLocation(string larsCode)
    {
        SetupController();
        SetLocationOptionInSession(LocationOption.EmployerLocation);
        SetProviderCourseLocationsInMediatorResponse(
        [
            new ProviderCourseLocation { LocationType = LocationType.Provider }
        ]);
        ConfirmNationalProviderSubmitModel model = new()
        {
            HasNationalDeliveryOption = true
        };

        await Sut.Index(larsCode, model);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteProviderLocations), It.IsAny<CancellationToken>()));
    }

    [Test, AutoData]
    public async Task WhenAddingNationLocation_ThenAddingNationalLocation_AndDeletesSessionModel(string larsCode, LocationOption locationOption, LocationType locationType)
    {
        SetupController();
        SetLocationOptionInSession(locationOption);
        SetProviderCourseLocationsInMediatorResponse(
        [
            new ProviderCourseLocation { LocationType = locationType }
        ]);
        ConfirmNationalProviderSubmitModel model = new()
        {
            HasNationalDeliveryOption = true
        };

        await Sut.Index(larsCode, model);

        MediatorMock.Verify(m => m.Send(It.IsAny<AddNationalLocationToStandardCommand>(), It.IsAny<CancellationToken>()));
        SessionServiceMock.Verify(s => s.Delete(It.IsAny<EditLocationOptionsSessionModel>()), Times.Once);
    }

    [Test, AutoData]
    public async Task WhenAddingNationLocation_ThenAddingNationalLocation_AndRedirectsToStandardDetails(string larsCode, LocationOption locationOption, LocationType locationType)
    {
        SetupController();
        SetLocationOptionInSession(locationOption);
        SetProviderCourseLocationsInMediatorResponse(
        [
            new ProviderCourseLocation { LocationType = locationType }
        ]);
        ConfirmNationalProviderSubmitModel model = new()
        {
            HasNationalDeliveryOption = true
        };

        var result = await Sut.Index(larsCode, model);

        MediatorMock.Verify(m => m.Send(It.IsAny<AddNationalLocationToStandardCommand>(), It.IsAny<CancellationToken>()));
        result.Should().BeOfType<RedirectToRouteResult>().Which.RouteName.Should().Be(RouteNames.GetStandardDetails);
    }
}
