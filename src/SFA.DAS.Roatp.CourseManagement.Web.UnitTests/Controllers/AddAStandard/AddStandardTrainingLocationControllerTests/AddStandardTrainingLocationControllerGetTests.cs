using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.AddStandardTrainingLocationControllerTests
{
    [TestFixture]
    public class AddStandardTrainingLocationControllerGetTests
    {
        private int larsCode = 1;
        [Test, MoqAutoData]
        public async Task Get_ModelMissingFromSession_RedirectsToSelectAStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddStandardTrainingLocationController sut)
        {
             sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var result = await sut.SelectAProviderlocation();

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
        }

        [Test, MoqAutoData]
        public async Task Get_Model_ReturnsView(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] AddStandardTrainingLocationController sut,
            GetAllProviderLocationsQueryResult allLocations,
            string cancelLink)
        {

            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.GetNewStandardViewTrainingLocationOptions, cancelLink);
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(allLocations);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(new StandardSessionModel{LarsCode = larsCode});

            var result = await sut.SelectAProviderlocation();
        
            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(AddStandardTrainingLocationController.ViewPath);
            result.As<ViewResult>().Model.As<CourseLocationAddViewModel>().CancelLink.Should().Be(cancelLink);
            result.As<ViewResult>().Model.As<CourseLocationAddViewModel>().BackLink.Should().Be(cancelLink);
        }
    }
}
