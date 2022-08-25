using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.AddStandardAddRegionsControllerTests
{
    [TestFixture]
    public class AddStandardAddRegionsControllerGetTests
    {
        [Test, MoqAutoData]
        public async Task SelectRegions_StandardSessionModelMissing_RedirectsToSelectStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddStandardAddRegionsController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(TestConstants.DefaultUkprn)).Returns(default(StandardSessionModel));

            var result = await sut.SelectRegions();

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
        }

        [Test, MoqAutoData]
        public async Task SelectRegions_ReturnsView(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] AddStandardAddRegionsController sut,
            StandardSessionModel standardSessionModel,
            GetAllRegionsAndSubRegionsQueryResult queryResult)
        {
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.ViewStandards);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(TestConstants.DefaultUkprn)).Returns(standardSessionModel);
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRegionsAndSubRegionsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

            var result = await sut.SelectRegions();

            result.As<ViewResult>().ViewName.Should().Be(AddStandardAddRegionsController.ViewPath);
            result.As<ViewResult>().Model.As<AddStandardAddRegionsViewModel>().SubregionsGroupedByRegions.Should().NotBeEmpty();
            result.As<ViewResult>().Model.As<AddStandardAddRegionsViewModel>().CancelLink.Should().Be(TestConstants.DefaultUrl);
        }
    }
}
