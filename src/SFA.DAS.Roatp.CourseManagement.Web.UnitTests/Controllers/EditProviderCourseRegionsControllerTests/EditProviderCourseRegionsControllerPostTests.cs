using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllStandardRegions;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditProviderCourseRegionsControllerTests;

[TestFixture]
public class EditProviderCourseRegionsControllerPostTests
{
    private static string DetailsUrl = Guid.NewGuid().ToString();
    private Mock<ILogger<EditProviderCourseRegionsController>> _loggerMock;
    private Mock<IMediator> _mediatorMock;
    private Mock<IValidator<RegionsSubmitModel>> _validatorMock;
    private Mock<ISessionService> _sessionServiceMock;
    private EditProviderCourseRegionsController _sut;

    [SetUp]
    public void Before_Each_Test()
    {
        _loggerMock = new Mock<ILogger<EditProviderCourseRegionsController>>();
        _mediatorMock = new Mock<IMediator>();
        _validatorMock = new Mock<IValidator<RegionsSubmitModel>>();
        _sessionServiceMock = new Mock<ISessionService>();

        _sut = new EditProviderCourseRegionsController(_mediatorMock.Object, _loggerMock.Object, _validatorMock.Object, _sessionServiceMock.Object);
        _sut
            .AddDefaultContextWithUser()
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.GetStandardDetails, DetailsUrl);
    }

    [Test, AutoData]
    public async Task WhenUpdatingRegions_TheSubmitModelIsValid_ThenUpdatesRegions(string larsCode)
    {
        SetProviderCourseLocationsInMediatorResponse([new ProviderCourseLocation()]);
        RegionsSubmitModel model = new() { SelectedSubRegions = ["1", "2"] };

        _validatorMock.Setup(x => x.Validate(It.IsAny<RegionsSubmitModel>())).Returns(new ValidationResult());

        var result = await _sut.UpdateStandardSubRegions(larsCode, model);

        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateStandardSubRegionsCommand>(), It.IsAny<CancellationToken>()), Times.Once);

        var routeResult = result as RedirectToRouteResult;
        routeResult.Should().NotBeNull();
        routeResult.RouteName.Should().Be(RouteNames.GetStandardDetails);
        routeResult.RouteValues.Should().NotBeEmpty().And.HaveCount(2);
        routeResult.RouteValues.Should().ContainKey("ukprn").WhoseValue.Should().Be(Convert.ToInt32(TestConstants.DefaultUkprn));
        routeResult.RouteValues.Should().ContainKey("larsCode").WhoseValue.Should().Be(larsCode);
    }

    [Test, AutoData]
    public async Task WhenUpdatingRegions_TheSubmitModelIsInvalid_ThenReturnsViewWithErrors(GetAllStandardRegionsQueryResult queryResult, string larsCode)
    {
        var validationResult = new ValidationResult();

        validationResult.Errors.Add(new ValidationFailure("Field", "Error"));

        _validatorMock.Setup(x => x.Validate(It.IsAny<RegionsSubmitModel>())).Returns(validationResult);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllStandardRegionsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult);
        RegionsSubmitModel model = new() { SelectedSubRegions = null };

        var result = await _sut.UpdateStandardSubRegions(larsCode, model);

        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateStandardSubRegionsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        result.As<ViewResult>().ViewName.Should().Be(EditProviderCourseRegionsController.ViewPath);
        result.As<ViewResult>().Model.As<RegionsViewModel>().AllRegions.TrueForAll(r => r.IsSelected);
    }

    [Test, AutoData]
    public async Task WhenSavingRegions_AndNationalLocationExists_ThenDeletesNationalLocation(string larsCode)
    {
        SetProviderCourseLocationsInMediatorResponse([new ProviderCourseLocation() { LocationType = LocationType.National }]);
        _validatorMock.Setup(x => x.Validate(It.IsAny<RegionsSubmitModel>())).Returns(new ValidationResult());
        RegionsSubmitModel model = new() { SelectedSubRegions = ["1", "2"] };

        await _sut.UpdateStandardSubRegions(larsCode, model);

        _mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, AutoData]
    public async Task WhenSavingRegions_AndNationalLocationDoesNotExist_ThenDoesNotDeleteNationalLocation(string larsCode)
    {
        SetProviderCourseLocationsInMediatorResponse([new ProviderCourseLocation() { LocationType = LocationType.Provider }]);
        _validatorMock.Setup(x => x.Validate(It.IsAny<RegionsSubmitModel>())).Returns(new ValidationResult());
        RegionsSubmitModel model = new() { SelectedSubRegions = ["1", "2"] };

        await _sut.UpdateStandardSubRegions(larsCode, model);

        _mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestCase(LocationOption.EmployerLocation, LocationType.Provider, true)]
    [TestCase(LocationOption.EmployerLocation, LocationType.National, false)]
    [TestCase(LocationOption.EmployerLocation, LocationType.Regional, false)]
    [TestCase(LocationOption.Both, LocationType.Provider, false)]
    public async Task WhenSavingRegions_AndEditingLocationOptions_ThenDeletesProviderLocationWhereApplicable_AndClearsSession(LocationOption locationOption, LocationType locationType, bool isDeleteProviderLocationInvoked)
    {
        SetProviderCourseLocationsInMediatorResponse([new ProviderCourseLocation() { LocationType = locationType }]);
        _validatorMock.Setup(x => x.Validate(It.IsAny<RegionsSubmitModel>())).Returns(new ValidationResult());

        EditLocationOptionsSessionModel sessionModel = new() { UpdatedLocationOption = locationOption };
        _sessionServiceMock.Setup(s => s.Get<EditLocationOptionsSessionModel>()).Returns(sessionModel);

        RegionsSubmitModel model = new() { SelectedSubRegions = ["1", "2"] };

        // Act
        await _sut.UpdateStandardSubRegions("any_lars_code", model);

        _mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteProviderLocations), It.IsAny<CancellationToken>()), Times.Exactly(isDeleteProviderLocationInvoked ? 1 : 0));
        _sessionServiceMock.Verify(s => s.Delete(It.IsAny<EditLocationOptionsSessionModel>()), Times.Once);
    }

    [Test]
    public async Task WhenSavingRegions_AndNotEditingLocationOptions_ThenDoesNotDeleteProviderLocation()
    {
        SetProviderCourseLocationsInMediatorResponse([new ProviderCourseLocation() { LocationType = LocationType.Provider }]);
        _validatorMock.Setup(x => x.Validate(It.IsAny<RegionsSubmitModel>())).Returns(new ValidationResult());
        _sessionServiceMock.Setup(s => s.Get<EditLocationOptionsSessionModel>()).Returns((EditLocationOptionsSessionModel)null);
        RegionsSubmitModel model = new() { SelectedSubRegions = ["1", "2"] };

        // Act
        await _sut.UpdateStandardSubRegions("any_lars_code", model);

        _mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteProviderLocations), It.IsAny<CancellationToken>()), Times.Never);
        _sessionServiceMock.Verify(s => s.Delete(It.IsAny<EditLocationOptionsSessionModel>()), Times.Never);
    }

    protected GetProviderCourseDetailsQueryResult SetProviderCourseLocationsInMediatorResponse(List<ProviderCourseLocation> providerCourseLocations)
    {
        var result = new GetProviderCourseDetailsQueryResult { ProviderCourseLocations = providerCourseLocations };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProviderCourseDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        return result;
    }
}
