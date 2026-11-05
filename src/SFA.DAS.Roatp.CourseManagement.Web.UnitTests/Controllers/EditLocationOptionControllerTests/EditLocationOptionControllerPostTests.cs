using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditLocationOptionControllerTests;

[TestFixture]
public class EditLocationOptionControllerPostTests : EditLocationOptionControllerTestBase
{
    private const string LarsCode = "123";
    private const int Ukprn = 10012002;
    [Test]
    public async Task Post_InvalidModel_ThenReturnsViewWithValidationError()
    {
        var validationResult = new ValidationResult();

        validationResult.Errors.Add(new ValidationFailure("Field", "Error"));

        _validatorMock.Setup(x => x.Validate(It.IsAny<LocationOptionSubmitModel>())).Returns(validationResult);

        var model = new EditLocationOptionViewModel();
        _sut.ModelState.AddModelError("key", "message");

        // Action
        var result = await _sut.Index(LarsCode, Ukprn, model);

        result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<EditLocationOptionViewModel>().Which.LocationOption.Should().Be(LocationOption.None);
        _mediatorMock.Verify(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, AutoData]
    public async Task Post_ThenSetsSessionModel(ProviderCourseLocation providerCourseLocation, EditLocationOptionViewModel model)
    {
        GetProviderCourseDetailsQueryResult result = SetProviderCourseLocationsInMediatorResponse([providerCourseLocation]);

        await _sut.Index(LarsCode, Ukprn, model);

        _sessionServiceMock.Verify(s => s.Set(It.Is<EditLocationOptionsSessionModel>(m => m.ExistingLocationOption == result.LocationOption && m.HasExistingNationalOption == result.HasNationalLocation && m.UpdatedLocationOption == model.LocationOption)), Times.Once);
    }

    [TestCase(LocationType.National)]
    [TestCase(LocationType.Regional)]
    public async Task WhenChangingFromBothToProviderOption_ThenDeletesEmployerLocations(LocationType employerLocation)
    {
        SetProviderCourseLocationsInMediatorResponse(
        [
            new ProviderCourseLocation { LocationType = LocationType.Provider },
            new ProviderCourseLocation { LocationType = employerLocation }
        ]);
        EditLocationOptionViewModel model = new() { LocationOption = LocationOption.ProviderLocation };

        await _sut.Index(LarsCode, Ukprn, model);

        _mediatorMock.Verify(m => m.Send(
            It.Is<DeleteCourseLocationsCommand>(d => d.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations), It.IsAny<CancellationToken>()));
        // Verify no other deletes are invoked
        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteCourseLocationsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestCase(LocationType.National)]
    [TestCase(LocationType.Regional)]
    public async Task WhenChangingFromBothToEmployerOption_ThenDeletesProviderLocations(LocationType employerLocation)
    {
        SetProviderCourseLocationsInMediatorResponse(
        [
            new ProviderCourseLocation { LocationType = LocationType.Provider },
            new ProviderCourseLocation { LocationType = employerLocation }
        ]);
        EditLocationOptionViewModel model = new() { LocationOption = LocationOption.EmployerLocation };

        await _sut.Index(LarsCode, Ukprn, model);

        _mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(d => d.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteProviderLocations), It.IsAny<CancellationToken>()));
        // Verify no other deletes are invoked
        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteCourseLocationsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestCase(LocationType.National, LocationOption.ProviderLocation)]
    [TestCase(LocationType.Regional, LocationOption.ProviderLocation)]
    [TestCase(LocationType.Provider, LocationOption.EmployerLocation)]
    [TestCase(LocationType.Provider, LocationOption.ProviderLocation)]
    [TestCase(LocationType.National, LocationOption.EmployerLocation)]
    [TestCase(LocationType.Regional, LocationOption.EmployerLocation)]
    public async Task GivenCombinations_ThenDoesNotDeleteAnyLocation(LocationType locationType, LocationOption locationOption)
    {
        SetProviderCourseLocationsInMediatorResponse([new ProviderCourseLocation { LocationType = locationType }]);
        EditLocationOptionViewModel model = new() { LocationOption = locationOption };

        await _sut.Index(LarsCode, Ukprn, model);

        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteCourseLocationsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestCase(LocationOption.ProviderLocation, LocationType.National, RouteNames.GetProviderCourseLocations)]
    [TestCase(LocationOption.ProviderLocation, LocationType.Regional, RouteNames.GetProviderCourseLocations)]
    [TestCase(LocationOption.ProviderLocation, LocationType.Provider, RouteNames.GetProviderCourseLocations)]
    [TestCase(LocationOption.Both, LocationType.National, RouteNames.GetProviderCourseLocations)]
    [TestCase(LocationOption.Both, LocationType.Regional, RouteNames.GetProviderCourseLocations)]
    [TestCase(LocationOption.Both, LocationType.Provider, RouteNames.GetProviderCourseLocations)]
    [TestCase(LocationOption.EmployerLocation, LocationType.National, RouteNames.GetNationalDeliveryOption)]
    [TestCase(LocationOption.EmployerLocation, LocationType.Regional, RouteNames.GetNationalDeliveryOption)]
    [TestCase(LocationOption.EmployerLocation, LocationType.Provider, RouteNames.GetNationalDeliveryOption)]
    public async Task ThenRedirectsBasedOnOptionsSelected(LocationOption locationOption, LocationType locationType, string routeName)
    {
        SetProviderCourseLocationsInMediatorResponse([new ProviderCourseLocation { LocationType = locationType }]);
        EditLocationOptionViewModel model = new() { LocationOption = locationOption };

        var result = await _sut.Index("123", Ukprn, model);

        result.Should().BeOfType<RedirectToRouteResult>().Which.RouteName.Should().Be(routeName);
    }
}
