using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.ConfirmNonRegulatedStandardControllerTests
{
    [TestFixture]
    public class ConfirmNonRegulatedStandardControllerGetTests
    {
        [Test, MoqAutoData]
        public async Task Get_ModelMissingFromSession_RedirectsToReviewYourDetails(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNonRegulatedStandardController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var response = await sut.GetConfirmationOfStandard();

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.ReviewYourDetails);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Get_LarsCodeNotSetInSession_RedirectsToStandardList(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            StandardSessionModel sessionModel)
        {
            // LarsCode must be null/empty to Not Set In Session
            sessionModel.LarsCode = null;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

            var response = await sut.GetConfirmationOfStandard();

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.ReviewYourDetails);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Get_ModelInSessionWithLarsCode_ReturnsView(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            StandardSessionModel sessionModel,
            GetStandardInformationQueryResult standardInformation)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>())).ReturnsAsync(standardInformation);

            var response = await sut.GetConfirmationOfStandard();

            var result = (ViewResult)response;
            result.Should().NotBeNull();
            result.ViewName.Should().Be(ConfirmNonRegulatedStandardController.ViewPath);
            var viewModel = result.Model as ConfirmNonRegulatedStandardViewModel;
            viewModel.Should().NotBeNull();
            viewModel!.StandardInformation.Should().BeEquivalentTo(standardInformation, o => o.ExcludingMissingMembers());
        }
    }
}
