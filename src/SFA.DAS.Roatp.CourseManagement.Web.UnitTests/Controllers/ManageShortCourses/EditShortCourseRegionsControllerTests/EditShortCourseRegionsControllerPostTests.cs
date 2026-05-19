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
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseRegionsControllerTests;

public class EditShortCourseRegionsControllerPostTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_InvalidModelState_ReturnsView(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<IRegionsService> regionsService,
            [Greedy] EditShortCourseRegionsController sut,
            RegionsSubmitModel model,
            List<RegionModel> regions,
            string larsCode)
    {
        // Arrange
        var learningType = LearningType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        // Act
        var result = await sut.EditShortCourseRegions(model, learningType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var viewModel = viewResult.Model as SelectShortCourseRegionsViewModel;
        viewModel.SubregionsGroupedByRegions.Should().NotBeEmpty();
        viewModel.LearningType.Should().Be(learningType);
        viewModel.SubmitButtonText.Should().Be(ButtonText.Confirm);
        viewModel.Route.Should().Be(RouteNames.EditShortCourseRegions);
        viewModel.IsAddJourney.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_InvalidModelState_VerifyMediatorsAreInvokedCorrectly(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<IRegionsService> regionsService,
            [Greedy] EditShortCourseRegionsController sut,
            RegionsSubmitModel model,
            List<RegionModel> regions,
            string larsCode)
    {
        // Arrange
        var learningType = LearningType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        // Act
        await sut.EditShortCourseRegions(model, learningType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Never());
        regionsService.Verify(m => m.GetRegions(), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_ValidModelState_SendsUpdateCommandAndVerifyMediatorIsInvoked(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<RegionsSubmitModel>> validator,
        [Greedy] EditShortCourseRegionsController sut,
        RegionsSubmitModel model,
        string larsCode)
    {
        // Arrange
        var learningType = LearningType.ApprenticeshipUnit;
        model.SelectedSubRegions = ["1", "2"];

        validator.Setup(x => x.Validate(It.IsAny<RegionsSubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseRegions(model, learningType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        mediatorMock.Verify(m => m.Send(It.Is<UpdateStandardSubRegionsCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
    }
}
