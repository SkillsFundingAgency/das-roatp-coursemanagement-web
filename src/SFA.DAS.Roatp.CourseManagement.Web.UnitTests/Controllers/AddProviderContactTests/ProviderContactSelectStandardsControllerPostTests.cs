using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderContactTests;

[TestFixture]
public class SelectStandardsForUpdateControllerPostTests
{
    [Test, MoqAutoData]
    public void Post_ModelStateIsInvalid_ReturnsViewResult(
         [Frozen] Mock<ISessionService> sessionServiceMock,
         [Frozen] Mock<IMediator> mediatorMock,
         [Greedy] SelectStandardsForUpdateController sut,
         List<ProviderContactStandardModel> standards,
         int ukprn)
    {
        var submitViewModel = new AddProviderContactStandardsSubmitViewModel
        {
            SelectedProviderCourseIds = standards.Select(x => x.ProviderCourseId).ToList()
        };

        var sessionModel = new ProviderContactSessionModel
        {
            Standards = standards
        };

        var expectedShowStandards = sessionModel.Standards.Any(x => x.CourseType == CourseType.Apprenticeship);
        var expectedApprenticeshipUnits = sessionModel.Standards.Any(x => x.CourseType == CourseType.ShortCourse);

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);


        sut.ModelState.AddModelError("key", "message");

        var result = sut.PostStandards(ukprn, submitViewModel);

        var viewResult = result as ViewResult;
        var model = viewResult!.Model as AddProviderContactStandardsViewModel;
        model!.Standards.Should().BeEquivalentTo(standards.Where(x => x.CourseType == CourseType.Apprenticeship).ToList(), options => options.Excluding(x => x.CourseName));
        model!.ApprenticeshipUnits.Should().BeEquivalentTo(standards.Where(x => x.CourseType == CourseType.ShortCourse).ToList(), options => options.Excluding(x => x.CourseName));
        model!.ShowStandards.Should().Be(expectedShowStandards);
        model!.ShowApprenticeshipUnits.Should().Be(expectedApprenticeshipUnits);

        sessionServiceMock.Verify(s => s.Set(It.IsAny<ProviderContactSessionModel>()), Times.Once());
    }

    [Test, MoqAutoData]
    public void Post_RedirectsToPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<AddProviderContactStandardsSubmitViewModel>> validator,
        [Greedy] SelectStandardsForUpdateController sut,
        List<ProviderContactStandardModel> standards,
        int ukprn)
    {
        var providerCourseIds = standards.Select(x => x.ProviderCourseId).ToList();

        var submitViewModel = new AddProviderContactStandardsSubmitViewModel
        {
            SelectedProviderCourseIds = providerCourseIds
        };

        var sessionModel = new ProviderContactSessionModel
        {
            Standards = standards
        };
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);
        validator.Setup(x => x.Validate(It.IsAny<AddProviderContactStandardsSubmitViewModel>())).Returns(new ValidationResult());

        var result = sut.PostStandards(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContactCheckStandards);

        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ProviderContactSessionModel>()), Times.Once());
    }

    [Test, MoqAutoData]
    public void Post_NoSession_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] SelectStandardsForUpdateController sut,
        List<ProviderContactStandardModel> standards,
        int ukprn)
    {
        var submitViewModel = new AddProviderContactStandardsSubmitViewModel
        {
            SelectedProviderCourseIds = standards.Select(x => x.ProviderCourseId).ToList()
        };

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns((ProviderContactSessionModel)null);

        var result = sut.PostStandards(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);

        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ProviderContactSessionModel>()), Times.Never());
    }
}
