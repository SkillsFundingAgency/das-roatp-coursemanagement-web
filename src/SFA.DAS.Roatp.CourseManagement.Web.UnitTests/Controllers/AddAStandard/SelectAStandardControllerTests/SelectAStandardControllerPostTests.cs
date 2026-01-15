using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.SelectAStandardControllerTests;

[TestFixture]
public class SelectAStandardControllerPostTests
{
    [Test, MoqAutoData]
    public async Task SubmitAStandard_InvalidState_ReturnsViewResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] SelectAStandardController sut,
        GetAvailableProviderStandardsQueryResult queryResult)
    {
        sut
            .AddDefaultContextWithUser()
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ViewStandards);
        sut.ModelState.AddModelError("key", "message");
        mediatorMock.Setup(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.Apprenticeship), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        var response = await sut.SubmitAStandard(new SelectAStandardSubmitModel());

        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        viewResult.ViewName.Should().Be(SelectAStandardController.ViewPath);
        var model = viewResult.Model as SelectAStandardViewModel;
        var expectedNames = queryResult.AvailableCourses.Select(s => $"{s.Title} (Level {s.Level})");
        model.Standards.All(s => expectedNames.Contains(s.Text)).Should().BeTrue();
        mediatorMock.Verify(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.Apprenticeship), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task SubmitAStandard_IfNotRegulatedForProvider_RedirectsToRespectiveConfirmationPage(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectAStandardController sut,
        SelectAStandardSubmitModel submitModel,
        GetStandardInformationQueryResult standardInformation)
    {
        standardInformation.IsRegulatedForProvider = false;
        mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(g => g.LarsCode == submitModel.SelectedLarsCode), It.IsAny<CancellationToken>())).ReturnsAsync(standardInformation);
        sut
            .AddDefaultContextWithUser()
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ViewStandards);

        var response = await sut.SubmitAStandard(submitModel);

        var result = response as RedirectToRouteResult;
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.GetAddStandardConfirmNonRegulatedStandard);
        sessionServiceMock.Verify(s => s.Set(It.Is<StandardSessionModel>(m => m.LarsCode == submitModel.SelectedLarsCode)));
        mediatorMock.Verify(m => m.Send(It.IsAny<GetAvailableProviderStandardsQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task SubmitAStandard_IfRegulatedStandard_RedirectsToRespectiveConfirmationPage(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectAStandardController sut,
        SelectAStandardSubmitModel submitModel,
        GetStandardInformationQueryResult standardInformation)
    {
        standardInformation.IsRegulatedForProvider = true;
        mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(g => g.LarsCode == submitModel.SelectedLarsCode), It.IsAny<CancellationToken>())).ReturnsAsync(standardInformation);
        sut
            .AddDefaultContextWithUser()
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ViewStandards);

        var response = await sut.SubmitAStandard(submitModel);

        var result = response as RedirectToRouteResult;
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.GetAddStandardConfirmRegulatedStandard);
        sessionServiceMock.Verify(s => s.Set(It.Is<StandardSessionModel>(m => m.LarsCode == submitModel.SelectedLarsCode)));
        mediatorMock.Verify(m => m.Send(It.IsAny<GetAvailableProviderStandardsQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}