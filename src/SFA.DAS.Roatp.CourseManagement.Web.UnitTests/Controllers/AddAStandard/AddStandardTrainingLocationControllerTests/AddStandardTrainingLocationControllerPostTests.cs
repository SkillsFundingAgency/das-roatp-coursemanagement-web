using System;
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
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.AddStandardTrainingLocationControllerTests
{
    [TestFixture]
    public class AddStandardTrainingLocationControllerPostTests
    {
        private int larsCode = 1;

        [Test, MoqAutoData]
        public async Task Post_ModelMissingFromSession_RedirectsToReviewYourDetails(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddStandardTrainingLocationController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>())
                .Returns((StandardSessionModel)null);

            var result = await sut.SubmitAProviderlocation(new CourseLocationAddSubmitModel());

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ReviewYourDetails);
        }

        [Test, MoqAutoData]
        public async Task Get_ModelStateIsInvalid_ReturnsViewResult(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddStandardTrainingLocationController sut,
            StandardSessionModel standardSessionModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>())
                .Returns(standardSessionModel);
            sut.ModelState.AddModelError("key", "message");

            var result = await sut.SubmitAProviderlocation(new CourseLocationAddSubmitModel());

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(AddStandardTrainingLocationController.ViewPath);
        }

        [Test, MoqAutoData]
        public async Task Get_ModelStateIsValid_UpdatesStandardSessionModel(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddStandardTrainingLocationController sut,
            CourseLocationAddViewModel submitModel,
            string locationName)
        {
            var navigationId = Guid.NewGuid();
            var allLocations = new GetAllProviderLocationsQueryResult
            {
                ProviderLocations = new List<ProviderLocation>
                    { new ProviderLocation { LocationType = LocationType.Provider, LocationName = locationName, NavigationId = navigationId} }
            };

            submitModel.TrainingVenueNavigationId = navigationId.ToString();
            sut.AddDefaultContextWithUser();
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(allLocations);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(new StandardSessionModel { LarsCode = larsCode });

            var result = await sut.SubmitAProviderlocation(submitModel);

            sessionServiceMock.Verify(s =>
                s.Set(It.Is<StandardSessionModel>(x => x.CourseLocations.Any(
                    c => c.ProviderLocationId == navigationId && c.LocationName == locationName)
                )));

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetNewStandardViewTrainingLocationOptions);
        }
    }
}
