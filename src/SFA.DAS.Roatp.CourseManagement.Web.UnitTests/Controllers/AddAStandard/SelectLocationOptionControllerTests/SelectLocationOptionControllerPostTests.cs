﻿using AutoFixture.NUnit3;
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
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);
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
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.ViewStandards, cancelLink);
            sut.ModelState.AddModelError("key", "message");

            var result = sut.SubmitLocationOption(new LocationOptionSubmitModel());

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(SelectLocationOptionController.ViewPath);
            result.As<ViewResult>().Model.As<SelectLocationOptionViewModel>().Should().NotBeNull();
            result.As<ViewResult>().Model.As<SelectLocationOptionViewModel>().CancelLink.Should().Be(cancelLink);
        }

        [Test, MoqAutoData]
        public void SubmitLocationOption_LocationOptionIsEmployer_NavigateToConfirmNationalProvider(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] SelectLocationOptionController sut,
            StandardSessionModel sessionModel)
        {
            var locationOption = LocationOption.EmployerLocation;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            sut.AddDefaultContextWithUser();

            var result = sut.SubmitLocationOption(new LocationOptionSubmitModel { LocationOption = locationOption });

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardConfirmNationalProvider);
            sessionServiceMock.Verify(s => s.Set(It.IsAny<StandardSessionModel>()),Times.Once);
        }

        [Test, MoqAutoData]
        public void SubmitLocationOption_LocationOptionIsProvider_NavigateToViewTrainingLocations(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] SelectLocationOptionController sut,
            StandardSessionModel sessionModel)
        {
            var locationOption = LocationOption.ProviderLocation;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            sut.AddDefaultContextWithUser();

            var result = sut.SubmitLocationOption(new LocationOptionSubmitModel { LocationOption = locationOption });

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetNewStandardViewTrainingLocationOptions);
            sessionServiceMock.Verify(s => s.Set(It.IsAny<StandardSessionModel>()), Times.Once);
        }
    }
}
