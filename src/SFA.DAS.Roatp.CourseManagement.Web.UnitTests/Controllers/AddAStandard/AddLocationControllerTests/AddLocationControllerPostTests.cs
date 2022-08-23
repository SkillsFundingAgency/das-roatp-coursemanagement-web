using System;
using System.Collections.Generic;
using System.Text;
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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.AddLocationControllerTests
{
    [TestFixture]
    public class AddLocationControllerPostTests
    {
        private int larsCode = 1;

        [Test, MoqAutoData]
        public async Task Post_ModelMissingFromSession_RedirectsToSelectAStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddLocationController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>()))
                .Returns((StandardSessionModel)null);

            var result = await sut.SubmitAProviderlocation(new CourseLocationAddSubmitModel(), larsCode);

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
        }

        [Test, MoqAutoData]
        public async Task Get_ModelStateIsInvalid_ReturnsViewResult(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddLocationController sut,
            StandardSessionModel standardSessionModel,
            string cancelLink)
        {
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.GetNewStandardViewTrainingLocationOptions, cancelLink);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>()))
                .Returns(standardSessionModel);
            sut.ModelState.AddModelError("key", "message");

            var result = await sut.SubmitAProviderlocation(new CourseLocationAddSubmitModel(), larsCode);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(AddLocationController.ViewPath);
            result.As<ViewResult>().Model.As<CourseLocationAddViewModel>().CancelLink.Should().Be(cancelLink);
        }

        [Test, MoqAutoData]
        public async Task Get_ModelStateIsValid_UpdatesStandardSessionModel(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddLocationController sut,
            StandardSessionModel standardSessionModel,
            CourseLocationAddViewModel submitModel,
            string cancelLink)
        {
            var navigationId = Guid.NewGuid();
            var allLocations = new GetAllProviderLocationsQueryResult
            {
                ProviderLocations = new List<ProviderLocation>
                    { new ProviderLocation { LocationType = LocationType.Provider, LocationName = "location name",NavigationId = navigationId} }
            };

            submitModel.TrainingVenueNavigationId = navigationId.ToString();
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.GetNewStandardViewTrainingLocationOptions, cancelLink);
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(allLocations);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(new StandardSessionModel { LarsCode = larsCode });

            var result = await sut.SubmitAProviderlocation(submitModel,larsCode);

            sessionServiceMock.Verify(s => s.Set(It.IsAny<StandardSessionModel>(), TestConstants.DefaultUkprn));

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetNewStandardViewTrainingLocationOptions);
        }
    }
}
