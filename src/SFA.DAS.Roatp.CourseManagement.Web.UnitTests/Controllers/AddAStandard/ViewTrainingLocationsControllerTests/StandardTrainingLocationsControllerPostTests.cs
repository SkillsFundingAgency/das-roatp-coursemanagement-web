using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
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
        public void ViewTrainingLocations_SessionNotAvailable_RedirectsToReviewYourDetails(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] StandardTrainingLocationsController sut,
            TrainingLocationListViewModel model)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var result = sut.SubmitTrainingLocations(model);

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ReviewYourDetails);
        }

        [Test, MoqAutoData]
        public void Submit_ModelStateIsInvalid_ReturnsView(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] StandardTrainingLocationsController sut,
            StandardSessionModel sessionModel)
        {
            sessionModel.CourseLocations = new List<CourseLocationModel>();

            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            sut.AddDefaultContextWithUser();
            var result = sut.SubmitTrainingLocations(new TrainingLocationListViewModel());

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(StandardTrainingLocationsController.ViewPath);
            result.As<ViewResult>().Model.As<TrainingLocationListViewModel>().Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public void Submit_LocationOptionSetToProviders_RedirectsToReviewPage(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] StandardTrainingLocationsController sut,
            StandardSessionModel sessionModel)
        {
            sessionModel.LocationOption = LocationOption.ProviderLocation;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            sut.AddDefaultContextWithUser();

            var result = sut.SubmitTrainingLocations(new TrainingLocationListViewModel());

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardReviewStandard);
        }

        [Test, MoqAutoData]
        public void Submit_LocationOptionSetToBoth_RedirectsToNationalQuestionPage(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] StandardTrainingLocationsController sut,
            StandardSessionModel sessionModel)
        {
            sessionModel.LocationOption = LocationOption.Both;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            sut.AddDefaultContextWithUser();

            var result = sut.SubmitTrainingLocations(new TrainingLocationListViewModel());

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardConfirmNationalProvider);
        }
    }
}
