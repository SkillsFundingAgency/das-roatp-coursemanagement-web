using System;
using System.Collections.Generic;
using System.Linq;
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
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseLocationAddControllerTests;

[TestFixture]
public class ProviderCourseLocationAddPostControllerTests
{
    private const string Ukprn = "10012002";
    private Mock<IMediator> _mediatorMock;
    private Mock<IValidator<ProviderCourseLocationAddSubmitModel>> _validatorMock;
    private Mock<ISessionService> _sessionServiceMock;
    private ProviderCourseLocationAddController _sut;
    readonly string verifyUrl = "http://test";

    [SetUp]
    public void Before_Each_Test()
    {
        _mediatorMock = new Mock<IMediator>();
        _validatorMock = new Mock<IValidator<ProviderCourseLocationAddSubmitModel>>();
        _sessionServiceMock = new Mock<ISessionService>();
        _sut = new ProviderCourseLocationAddController(Mock.Of<ILogger<ProviderCourseLocationAddController>>(), _mediatorMock.Object, _validatorMock.Object, _sessionServiceMock.Object);
        _sut.AddDefaultContextWithUser()
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.GetProviderCourseLocations, verifyUrl);
    }

    [Test, AutoData]
    public async Task Post_ValidModel_RedirectToRoute(GetAllProviderLocationsQueryResult resultAllProviderLocations,
        GetProviderCourseLocationsQueryResult resultProviderCourseLocations, string larsCode, ProviderCourseLocationAddSubmitModel model)
    {
        resultAllProviderLocations.ProviderLocations[0].NavigationId = Guid.NewGuid();
        _mediatorMock
           .Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn)), It.IsAny<CancellationToken>()))
           .ReturnsAsync(resultAllProviderLocations);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultProviderCourseLocations);

        _validatorMock.Setup(x => x.Validate(It.IsAny<ProviderCourseLocationAddSubmitModel>())).Returns(new ValidationResult());

        model.TrainingVenueNavigationId = resultAllProviderLocations.ProviderLocations.First().NavigationId.ToString();
        var result = await _sut.SubmitAProviderlocation(larsCode, model);

        _mediatorMock.Verify(m => m.Send(It.IsAny<AddProviderCourseLocationCommand>(), It.IsAny<CancellationToken>()), Times.Once);

        var actual = (RedirectToRouteResult)result;
        Assert.NotNull(actual);
        actual.RouteName.Should().Be(RouteNames.GetProviderCourseLocations);
    }

    [Test, AutoData]
    public async Task Post_InValidModel_ReturnsSameView(GetAvailableProviderLocationsQueryResult availableProviderLocationsQueryResult, string larsCode, ProviderCourseLocationAddSubmitModel model)
    {
        _mediatorMock
        .Setup(m => m.Send(It.Is<GetAvailableProviderLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
        .ReturnsAsync(availableProviderLocationsQueryResult);

        var validationResult = new ValidationResult();

        validationResult.Errors.Add(new ValidationFailure("Field", "Error"));

        _validatorMock.Setup(x => x.Validate(It.IsAny<ProviderCourseLocationAddSubmitModel>())).Returns(validationResult);

        var result = await _sut.SubmitAProviderlocation(larsCode, model);

        _mediatorMock.Verify(m => m.Send(It.IsAny<AddProviderCourseLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Contain(ProviderCourseLocationAddController.ViewPath);
        var viewModel = viewResult.Model as ProviderCourseLocationAddViewModel;
        viewModel.Should().NotBeNull();
    }

    [Test, AutoData]
    public async Task WhenAddingProviderLocationToCourse_AndLocationOptionIsUpdatedToProvider_AndCourseHasEmployerLocation_ThenDeletesEmployerLocation(
        GetAllProviderLocationsQueryResult resultAllProviderLocations,
        string larsCode)
    {
        resultAllProviderLocations.ProviderLocations[0].NavigationId = Guid.NewGuid();
        _mediatorMock
           .Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn)), It.IsAny<CancellationToken>()))
           .ReturnsAsync(resultAllProviderLocations);
        SetProviderCourseLocationsInMediatorResponse([new ProviderCourseLocation { LocationType = LocationType.National }]);
        _validatorMock.Setup(x => x.Validate(It.IsAny<ProviderCourseLocationAddSubmitModel>())).Returns(new ValidationResult());
        ProviderCourseLocationAddSubmitModel model = new()
        {
            TrainingVenueNavigationId = resultAllProviderLocations.ProviderLocations[0].NavigationId.ToString()
        };
        // Set up session to have update location option set to provider and has employer locations
        var sessionModel = new EditLocationOptionsSessionModel
        {
            UpdatedLocationOption = LocationOption.ProviderLocation
        };
        _sessionServiceMock.Setup(s => s.Get<EditLocationOptionsSessionModel>()).Returns(sessionModel);


        // Act
        await _sut.SubmitAProviderlocation(larsCode, model);

        _mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations), It.IsAny<CancellationToken>()), Times.Once);
        _sessionServiceMock.Verify(s => s.Delete(It.IsAny<EditLocationOptionsSessionModel>()), Times.Once);
    }

    [Test, AutoData]
    public async Task WhenAddingProviderLocationToCourse_AndLocationOptionIsUpdatedToProvider_AndCourseDontHaveEmployerLocation_ThenDoesNotInvokeDeleteEmployerLocation(
       GetAllProviderLocationsQueryResult resultAllProviderLocations,
       string larsCode,
       ProviderCourseLocationAddSubmitModel model)
    {
        resultAllProviderLocations.ProviderLocations[0].NavigationId = Guid.NewGuid();
        _mediatorMock
           .Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn)), It.IsAny<CancellationToken>()))
           .ReturnsAsync(resultAllProviderLocations);
        SetProviderCourseLocationsInMediatorResponse([new ProviderCourseLocation { LocationType = LocationType.Provider }]);
        _validatorMock.Setup(x => x.Validate(It.IsAny<ProviderCourseLocationAddSubmitModel>())).Returns(new ValidationResult());
        model.TrainingVenueNavigationId = resultAllProviderLocations.ProviderLocations[0].NavigationId.ToString();
        // Set up session to have update location option set to provider and has employer locations
        var sessionModel = new EditLocationOptionsSessionModel
        {
            UpdatedLocationOption = LocationOption.ProviderLocation
        };
        _sessionServiceMock.Setup(s => s.Get<EditLocationOptionsSessionModel>()).Returns(sessionModel);


        // Act
        await _sut.SubmitAProviderlocation(larsCode, model);

        _mediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations), It.IsAny<CancellationToken>()), Times.Never);
        _sessionServiceMock.Verify(s => s.Delete(It.IsAny<EditLocationOptionsSessionModel>()), Times.Once);
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
