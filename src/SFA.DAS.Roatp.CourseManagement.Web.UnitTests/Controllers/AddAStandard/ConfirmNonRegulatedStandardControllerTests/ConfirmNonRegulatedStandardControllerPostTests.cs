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
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.ConfirmNonRegulatedStandardControllerTests
{
    [TestFixture]
    public class ConfirmNonRegulatedStandardControllerPostTests
    {
        [Test, MoqAutoData]
        public async Task Post_ModelMissingFromSession_RedirectsToStandardList(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            ConfirmNonRegulatedStandardSubmitModel submitModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var response = await sut.SubmitConfirmationOfStandard(submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Post_ModelStateIsInvalid_ReturnsViewResult(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            ConfirmNonRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            sut.ModelState.AddModelError("key", "message");

            var response = await sut.SubmitConfirmationOfStandard(submitModel);

            var result = (ViewResult)response;
            result.Should().NotBeNull();
            result.ViewName.Should().Be(ConfirmNonRegulatedStandardController.ViewPath);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public async Task Post_IsNotCorrectStandard_RedirectsToSelectStandard(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            ConfirmNonRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel)
        {
            submitModel.IsCorrectStandard = false;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

            var response = await sut.SubmitConfirmationOfStandard(submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Post_StandardConfirmed_UpdatesSessionModel(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            ConfirmNonRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel,
            GetStandardInformationQueryResult getStandardInformationQueryResult)
        {
            mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>())).ReturnsAsync(getStandardInformationQueryResult);
            submitModel.IsCorrectStandard = true;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

            await sut.SubmitConfirmationOfStandard(submitModel);

            sessionServiceMock.Verify(m => m.Set(sessionModel), Times.Once);
            sessionModel.IsConfirmed.Should().BeTrue();
            sessionModel.StandardInformation.Should().BeEquivalentTo(getStandardInformationQueryResult, option => option
                .Excluding(c => c.StandardUId)
                .Excluding(c => c.Regulated)
                .Excluding(c => c.IsRegulatedForProvider)
                .WithMapping<StandardInformationViewModel>(c => c.Title, v => v.CourseName));
        }

        [Test, MoqAutoData]
        public async Task Post_StandardConfirmed_RedirectsToContactDetails(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            ConfirmNonRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel)
        {
            submitModel.IsCorrectStandard = true;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

            var response = await sut.SubmitConfirmationOfStandard(submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetAddStandardAddContactDetails);
            sessionServiceMock.Verify(m => m.Set(It.IsAny<StandardSessionModel>()), Times.Once);
        }
    }
}
