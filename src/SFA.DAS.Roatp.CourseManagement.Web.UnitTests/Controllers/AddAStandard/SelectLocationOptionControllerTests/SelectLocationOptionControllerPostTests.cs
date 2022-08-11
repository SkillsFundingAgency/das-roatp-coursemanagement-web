using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.SelectLocationOptionControllerTests
{
    [TestFixture]
    public class SelectLocationOptionControllerPostTests
    {
        [Test, MoqAutoData]
        public void SubmitLocationOption_SessionModelNotFound_RedirectsToSelectStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] SelectLocationOptionController sut)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns((StandardSessionModel)null);
            sut.AddDefaultContextWithUser();

            var result = sut.SubmitLocationOption(new LocationOptionSubmitModel());

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
        }

        [Test, MoqAutoData]
        public void SubmitLocationOption_ModelStateIsInvalid_ReturnsView(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] SelectLocationOptionController sut,
            StandardSessionModel sessionModel,
            string cancelLink)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(sessionModel);
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.ViewStandards, cancelLink);
            sut.ModelState.AddModelError("key", "message");

            var result = sut.SubmitLocationOption(new LocationOptionSubmitModel());

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(SelectLocationOptionController.ViewPath);
            result.As<ViewResult>().Model.As<SelectLocationOptionViewModel>().Should().NotBeNull();
            result.As<ViewResult>().Model.As<SelectLocationOptionViewModel>().CancelLink.Should().Be(cancelLink);
        }

        [Test, MoqAutoData]
        public void SubmitLocationOption_ModelStateIsValid_UpdatesSessionModel(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] SelectLocationOptionController sut,
            StandardSessionModel sessionModel)
        {
            var locationOption = LocationOption.ProviderLocation;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(sessionModel);
            sut.AddDefaultContextWithUser();

            sut.SubmitLocationOption(new LocationOptionSubmitModel { LocationOption = locationOption});

            sessionServiceMock.Verify(s => s.Set(It.Is<StandardSessionModel>(m => m.LocationOption == locationOption), It.IsAny<string>()));
        }
    }
}
