using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

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
            ConfirmNonRegulatedStandardSubmitModel submitModel,
            int ukprn)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var response = await sut.SubmitConfirmationOfStandard(ukprn, submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.ReviewYourDetails);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardInformationQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Post_ModelStateIsInvalid_ReturnsViewResult(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            ConfirmNonRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel,
            int ukprn)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            sut.ModelState.AddModelError("key", "message");

            var response = await sut.SubmitConfirmationOfStandard(ukprn, submitModel);

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
            StandardSessionModel sessionModel,
            int ukprn)
        {
            submitModel.IsCorrectStandard = false;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

            var response = await sut.SubmitConfirmationOfStandard(ukprn, submitModel);

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
            GetStandardInformationQueryResult getStandardInformationQueryResult,
            int ukprn)
        {
            mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>())).ReturnsAsync(getStandardInformationQueryResult);
            submitModel.IsCorrectStandard = true;
            sut.AddDefaultContextWithUser();
            sessionModel.LatestProviderContactModel = null;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            mediatorMock.Setup(m => m.Send(It.IsAny<GetLatestProviderContactQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetLatestProviderContactQueryResult)null);

            await sut.SubmitConfirmationOfStandard(ukprn, submitModel);

            sessionServiceMock.Verify(m => m.Set(sessionModel), Times.Once);
            sessionModel.IsConfirmed.Should().BeTrue();
            sessionModel.StandardInformation.Should().BeEquivalentTo(getStandardInformationQueryResult, option => option
                .Excluding(c => c.StandardUId)
                .Excluding(c => c.IsRegulatedForProvider)
                .WithMapping<StandardInformationViewModel>(c => c.Title, v => v.CourseName)
                .WithMapping<StandardInformationViewModel>(c => c.ApprovalBody, v => v.RegulatorName)
                .WithMapping<StandardInformationViewModel>(c => c.Route, v => v.Sector));
        }

        [Test, MoqAutoData]
        public async Task Post_StandardConfirmed_RedirectsToContactDetails(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            ConfirmNonRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel,
            int ukprn)
        {
            submitModel.IsCorrectStandard = true;
            sut.AddDefaultContextWithUser();
            sessionModel.LatestProviderContactModel = null;
            sessionModel.IsUsingSavedContactDetails = null;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            mediatorMock.Setup(m => m.Send(It.IsAny<GetLatestProviderContactQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetLatestProviderContactQueryResult)null);

            var response = await sut.SubmitConfirmationOfStandard(ukprn, submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetAddStandardAddContactDetails);
            sessionServiceMock.Verify(m => m.Set(It.IsAny<StandardSessionModel>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Post_StandardConfirmed_HasContactDetails_RedirectsUseSavedContactDetails(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            ConfirmNonRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel,
            GetLatestProviderContactQueryResult contactQueryResult,
            ProviderContactModel latestProviderContactModel,
            int ukprn)
        {
            submitModel.IsCorrectStandard = true;
            sut.AddDefaultContextWithUser();
            sessionModel.LatestProviderContactModel = latestProviderContactModel;
            sessionModel.IsUsingSavedContactDetails = null;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            mediatorMock.Setup(m => m.Send(It.IsAny<GetLatestProviderContactQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(contactQueryResult);

            var response = await sut.SubmitConfirmationOfStandard(ukprn, submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetAddStandardUseSavedContactDetails);
        }
    }
}
