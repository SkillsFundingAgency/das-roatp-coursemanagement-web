using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.RemoveProviderCourseLocationControllerTests
{
    [TestFixture]
    public class RemoveProviderCourseLocationControllerTests
    {
        [Test, MoqAutoData]
        public void ModelMissingFromSession_RedirectsToSelectAStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] RemoveProviderCourseLocationController sut)
        {
            var providerLocationId = Guid.NewGuid();
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>())
                .Returns((StandardSessionModel)null);

            var result =  sut.RemoveProviderCourseLocation(providerLocationId);

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
        }

        [Test, MoqAutoData]
        public void ProviderLocationIdNotInCourseLocations_SessionNotUpdated(
            StandardSessionModel standardSessionModel)
        {
            var sessionServiceMock = new Mock<ISessionService>();
            var ctrl = new RemoveProviderCourseLocationController(sessionServiceMock.Object, Mock.Of<ILogger<RemoveProviderCourseLocationController>>());
            ctrl.AddDefaultContextWithUser();

            var providerLocationId = Guid.NewGuid();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
            var result = ctrl.RemoveProviderCourseLocation(providerLocationId);

            sessionServiceMock.Verify(s =>
                s.Set(It.IsAny<StandardSessionModel>()), Times.Never);

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetNewStandardViewTrainingLocationOptions);
        }

        [Test]
        public void ProviderLocationIdInCourseLocations_SessionUpdatedToRemoveProviderCourseLocation()
        {
            var ukprn = "10012002";
            var providerLocationId = Guid.NewGuid();
            var standardSessionModel = new StandardSessionModel
            {
                LarsCode = 1,
                CourseLocations = new List<CourseLocationModel>
                {
                    new CourseLocationModel
                    {
                        LocationType = LocationType.Provider,
                        ProviderLocationId = providerLocationId
                    },
                    new CourseLocationModel
                    {
                        LocationType = LocationType.Regional,
                        RegionName = "test",
                        ProviderLocationId = Guid.NewGuid()
                    }
                }
            };

            var sessionServiceMock = new Mock<ISessionService>();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
            var ctrl = new RemoveProviderCourseLocationController(sessionServiceMock.Object, Mock.Of<ILogger<RemoveProviderCourseLocationController>>());
            ctrl.AddDefaultContextWithUser();

            var result = ctrl.RemoveProviderCourseLocation(providerLocationId);

            sessionServiceMock.Verify(s => s.Set(standardSessionModel), Times.Once);

            standardSessionModel.CourseLocations.Count.Should().Be(1);
            standardSessionModel.CourseLocations.Any(x => x.ProviderLocationId == providerLocationId).Should()
                .BeFalse();
            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetNewStandardViewTrainingLocationOptions);
        }
    }
}
