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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.AddRegulatedStandardControllerTests
{
    [TestFixture]
    public class AddRegulatedStandardControllerPostTests
    {
        [Test, MoqAutoData]
        public async Task Post_ModelMissingFromSession_RedirectsToStandardList(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddRegulatedStandardController sut,
            ConfirmNewRegulatedStandardSubmitModel submitModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns((StandardSessionModel)null);

            var response = await sut.SubmitConfirmationOfRegulatedStandard(submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Post_ModelStateIsInvalid_ReturnsViewResult(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddRegulatedStandardController sut,
            ConfirmNewRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(sessionModel);
            sut.ModelState.AddModelError("key", "message");
        
            var response = await sut.SubmitConfirmationOfRegulatedStandard(submitModel);
        
            var result = (ViewResult)response;
            result.Should().NotBeNull();
            result.ViewName.Should().Be(AddRegulatedStandardController.ViewPath);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()));
        }
        
        [Test, MoqAutoData]
        public async Task Post_IsNotCorrectStandard_RedirectsToSelectStandard(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddRegulatedStandardController sut,
            ConfirmNewRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel)
        {
            submitModel.IsCorrectStandard = false;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(sessionModel);
        
            var response = await sut.SubmitConfirmationOfRegulatedStandard(submitModel);
        
            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetNeedApprovalToDeliverRegulatedStandard);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Test, MoqAutoData]
        public async Task Post_IsCorrectStandard_RedirectsToContactDetails(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddRegulatedStandardController sut,
            ConfirmNewRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel)
        {
            submitModel.IsCorrectStandard = true;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(sessionModel);
        
            var response = await sut.SubmitConfirmationOfRegulatedStandard(submitModel);
        
            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetAddStandardAddContactDetails);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            sessionServiceMock.Verify(m => m.Set(It.IsAny<StandardSessionModel>(), It.IsAny<string>()), Times.Once);
        }
    }
}
