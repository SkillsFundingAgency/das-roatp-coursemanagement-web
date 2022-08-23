﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.ViewTrainingLocationsControllerTests
{
    [TestFixture]
    public class ViewTrainingLocationsControllerGetTests
    {
        [Test, MoqAutoData]
        public void ViewTrainingLocations_SessionNotAvailable_RedirectsToSelectStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ViewTrainingLocationsController sut,
            string cancelLink)
        {
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.ViewStandards, cancelLink);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns((StandardSessionModel)null);

            var result = sut.ViewTrainingLocations();

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
        }

        [Test, MoqAutoData]
        public void ViewTrainingLocations_ReturnsView(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ViewTrainingLocationsController sut,
            string cancelLink)
        {
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.GetAddStandardSelectLocationOption, cancelLink);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(new StandardSessionModel { LocationOption = LocationOption.ProviderLocation, LarsCode = 1 });

            var result = sut.ViewTrainingLocations();

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(ViewTrainingLocationsController.ViewPath);
            result.As<ViewResult>().Model.As<TrainingLocationListViewModel>().Should().NotBeNull();
            result.As<ViewResult>().Model.As<TrainingLocationListViewModel>().CancelLink.Should().Be(cancelLink);
            result.As<ViewResult>().Model.As<TrainingLocationListViewModel>().BackLink.Should().Be(cancelLink);
        }

        [Test, MoqAutoData]
        public void ViewTrainingLocations_MapProviderLocations(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ViewTrainingLocationsController sut,
            DeliveryMethodModel deliveryModel,
            string cancelLink)
        {
            var locationName = "location name";
            var courseLocationModel = new CourseLocationModel();
            courseLocationModel.LocationName = locationName;
            courseLocationModel.LocationType = LocationType.Provider;
            courseLocationModel.DeliveryMethod = deliveryModel;

            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.GetAddStandardSelectLocationOption, cancelLink);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(new StandardSessionModel { LocationOption = LocationOption.ProviderLocation, LarsCode = 1, CourseLocations = new List<CourseLocationModel> {courseLocationModel}});

            var result = sut.ViewTrainingLocations();
            var model = result.As<ViewResult>().Model.As<TrainingLocationListViewModel>();
            model.ProviderCourseLocations.Count.Should().Be(1);
            model.ProviderCourseLocations.First().LocationName.Should().Be(locationName);
            model.ProviderCourseLocations.First().LocationType.Should().Be(LocationType.Provider);
            model.ProviderCourseLocations.First().DeliveryMethod.Should().Be(deliveryModel);
        }
    }
}
