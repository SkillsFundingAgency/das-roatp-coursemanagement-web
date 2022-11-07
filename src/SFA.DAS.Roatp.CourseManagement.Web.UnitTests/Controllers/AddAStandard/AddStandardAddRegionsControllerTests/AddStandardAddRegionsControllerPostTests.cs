using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.AddStandardAddRegionsControllerTests
{
    [TestFixture]
    public class AddStandardAddRegionsControllerPostTests
    {
        [Test, MoqAutoData]
        public async Task SubmitRegions_StandardSessionModelMissing_RedirectsToSelectStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddStandardAddRegionsController sut,
            RegionsSubmitModel submitModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(default(StandardSessionModel));

            var result = await sut.SubmitRegions(submitModel);

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
        }

        [Test, MoqAutoData]
        public async Task SubmitRegions_ModelStateIsInvalid_ReturnsView(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] AddStandardAddRegionsController sut,
            StandardSessionModel standardSessionModel,
            RegionsSubmitModel submitModel,
            GetAllRegionsAndSubRegionsQueryResult queryResult)
        {
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.ViewStandards);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRegionsAndSubRegionsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
            sut.ModelState.AddModelError("key", "message");

            var result = await sut.SubmitRegions(submitModel);

            result.As<ViewResult>().ViewName.Should().Be(AddStandardAddRegionsController.ViewPath);
            result.As<ViewResult>().Model.As<AddStandardAddRegionsViewModel>().SubregionsGroupedByRegions.Should().NotBeEmpty();
            result.As<ViewResult>().Model.As<AddStandardAddRegionsViewModel>().CancelLink.Should().Be(TestConstants.DefaultUrl);
        }

        [Test, MoqAutoData]
        public async Task SubmitRegions_UpdatesCourseLocationInSessionModel(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] AddStandardAddRegionsController sut,
            StandardSessionModel standardSessionModel,
            GetAllRegionsAndSubRegionsQueryResult queryResult)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRegionsAndSubRegionsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
            standardSessionModel.CourseLocations.Clear();
            var submitModel = new RegionsSubmitModel();
            submitModel.SelectedSubRegions = queryResult.Regions.Select(r => r.Id.ToString()).ToArray();

            await sut.SubmitRegions(submitModel);

            standardSessionModel.CourseLocations.Count.Should().Be(submitModel.SelectedSubRegions.Length);
            sessionServiceMock.Verify(s => s.Set(standardSessionModel));
        }

        [Test, MoqAutoData]
        public async Task SubmitRegions_NavigatesToReviewPage(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] AddStandardAddRegionsController sut,
            StandardSessionModel standardSessionModel,
            RegionsSubmitModel submitModel,
            GetAllRegionsAndSubRegionsQueryResult queryResult)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRegionsAndSubRegionsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

            var result = await sut.SubmitRegions(submitModel);

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardReviewStandard);
        }
    }
}
