using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
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
    public class StandardTrainingLocationsControllerGetTests
    {
        [Test, MoqAutoData]
        public async Task ViewTrainingLocations_SessionNotAvailable_RedirectsToReviewYourDetails(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] StandardTrainingLocationsController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var result = await sut.ViewTrainingLocations();

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ReviewYourDetails);
        }

        [Test, MoqAutoData]
        public async Task ViewTrainingLocations_IncorrectLocationOptionInSession_RedirectsToStandardsListPage(
           [Frozen] Mock<ISessionService> sessionServiceMock,
           [Greedy] StandardTrainingLocationsController sut,
           StandardSessionModel standardSessionModel)
        {
            standardSessionModel.LocationOption = LocationOption.EmployerLocation;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);

            var result = await sut.ViewTrainingLocations();

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ViewStandards);
        }

        [Test, MoqAutoData]
        public async Task ViewTrainingLocations_ReturnsView(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] StandardTrainingLocationsController sut,
            GetAllProviderLocationsQueryResult providerLocations)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(new StandardSessionModel { LocationOption = LocationOption.ProviderLocation, LarsCode = "1" });

            mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocations);

            var result = await sut.ViewTrainingLocations();

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(StandardTrainingLocationsController.ViewPath);
            result.As<ViewResult>().Model.As<TrainingLocationListViewModel>().Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task ViewTrainingLocations_MapProviderLocations(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] StandardTrainingLocationsController sut,
            DeliveryMethodModel deliveryModel,
            GetAllProviderLocationsQueryResult providerLocations)
        {
            var locationName = "location name";
            var courseLocationModel = new CourseLocationModel();
            courseLocationModel.LocationName = locationName;
            courseLocationModel.LocationType = LocationType.Provider;
            courseLocationModel.DeliveryMethod = deliveryModel;

            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(new StandardSessionModel { LocationOption = LocationOption.ProviderLocation, LarsCode = "1", CourseLocations = new List<CourseLocationModel> { courseLocationModel } });

            mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocations);

            var result = await sut.ViewTrainingLocations();
            var model = result.As<ViewResult>().Model.As<TrainingLocationListViewModel>();
            model.ProviderCourseLocations.Count.Should().Be(1);
            model.ProviderCourseLocations.First().LocationName.Should().Be(locationName);
            model.ProviderCourseLocations.First().LocationType.Should().Be(LocationType.Provider);
            model.ProviderCourseLocations.First().DeliveryMethod.Should().Be(deliveryModel);
        }

        [Test, MoqAutoData]
        public async Task ViewTrainingLocations_LocationsNotAvailable_RedirectsToGetAddTrainingVenue(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] StandardTrainingLocationsController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(new StandardSessionModel { LocationOption = LocationOption.ProviderLocation, LarsCode = "1" });

            mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(new GetAllProviderLocationsQueryResult());

            var result = await sut.ViewTrainingLocations() as RedirectToRouteResult;

            result.RouteName.Should().Be(RouteNames.GetAddProviderLocation);
        }
    }
}
