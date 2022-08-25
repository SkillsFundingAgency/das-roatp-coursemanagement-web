using System.Collections.Generic;
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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.ViewTrainingLocationsControllerTests
{
    [TestFixture]
    public class StandardTrainingLocationsControllerPostTests
    {
        [Test, MoqAutoData]
        public void ViewTrainingLocations_SessionNotAvailable_RedirectsToSelectStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] StandardTrainingLocationsController sut,
            TrainingLocationListViewModel model,
            string cancelLink)
        {
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.GetAddStandardSelectLocationOption, cancelLink);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns((StandardSessionModel)null);

            var result = sut.SubmitTrainingLocations(model);

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
        }

        [Test, MoqAutoData]
        public void Submit_ModelStateIsInvalid_ReturnsView(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] StandardTrainingLocationsController sut,
            StandardSessionModel sessionModel,
            string cancelLink)
        {
            sessionModel.CourseLocations = new List<CourseLocationModel>();

            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(sessionModel);
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.GetAddStandardSelectLocationOption, cancelLink);
            var result = sut.SubmitTrainingLocations(new TrainingLocationListViewModel {CancelLink = cancelLink});

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(StandardTrainingLocationsController.ViewPath);
            result.As<ViewResult>().Model.As<TrainingLocationListViewModel>().Should().NotBeNull();
            result.As<ViewResult>().Model.As<TrainingLocationListViewModel>().CancelLink.Should().Be(cancelLink);
        }

        [Test, MoqAutoData]
        public void Submit_ReturnsView(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] StandardTrainingLocationsController sut,
            StandardSessionModel sessionModel,
            string cancelLink)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(sessionModel);
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.GetAddStandardSelectLocationOption, cancelLink);
           
            var result = sut.SubmitTrainingLocations(new TrainingLocationListViewModel());

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardReviewStandard);
        }
    }
}
