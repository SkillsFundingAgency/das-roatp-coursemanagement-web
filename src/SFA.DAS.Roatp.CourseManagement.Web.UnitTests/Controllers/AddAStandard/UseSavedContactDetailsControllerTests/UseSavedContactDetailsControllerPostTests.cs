using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.UseSavedContactDetailsControllerTests;

[TestFixture]
public class UseSavedContactDetailsControllerPostTests
{
    [Test, MoqAutoData]
    public void Get_ModelMissingFromSession_RedirectsToSelectAStandard(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] UseSavedContactDetailsController sut)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

        var result = sut.PostUseSavedContactDetails(new UseSavedContactDetailsViewModel { Ukprn = 12345678 });

        result.As<RedirectToRouteResult>().Should().NotBeNull();
        result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
    }

    [Test, MoqAutoData]
    public void Get_ModelStateIsInvalid_ReturnsViewResult(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] UseSavedContactDetailsController sut,
         StandardSessionModel standardSessionModel)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        sut.ModelState.AddModelError("key", "message");

        var result = sut.PostUseSavedContactDetails(new UseSavedContactDetailsViewModel { Ukprn = 12345678 });

        result.As<ViewResult>().Should().NotBeNull();
        result.As<ViewResult>().ViewName.Should().Be(UseSavedContactDetailsController.ViewPath);
    }

    [Test, MoqAutoData]
    public void Get_ModelStateIsValid_UpdatesStandardSessionModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] UseSavedContactDetailsController sut,
        StandardSessionModel standardSessionModel,
        UseSavedContactDetailsViewModel submitModel)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);

        sut.PostUseSavedContactDetails(submitModel);

        sessionServiceMock.Verify(s => s.Set(It.Is<StandardSessionModel>(m => m.IsUsingSavedContactDetails == submitModel.IsUsingSavedContactDetails)), Times.Once);
    }

    [Test, MoqAutoData]
    public void Get_ModelStateIsValid_RedirectToSelectLocationOption(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] UseSavedContactDetailsController sut,
        StandardSessionModel standardSessionModel,
        UseSavedContactDetailsViewModel submitModel)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);

        var result = sut.PostUseSavedContactDetails(submitModel);

        result.As<RedirectToRouteResult>().Should().NotBeNull();
        result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardAddContactDetails);
    }
}