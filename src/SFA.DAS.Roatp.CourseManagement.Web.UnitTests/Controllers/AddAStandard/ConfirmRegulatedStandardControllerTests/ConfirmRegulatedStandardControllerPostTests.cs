using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.ConfirmRegulatedStandardControllerTests
{
    [TestFixture]
    public class ConfirmRegulatedStandardControllerPostTests
    {
        [Test, MoqAutoData]
        public async Task Post_ModelMissingFromSession_RedirectsToStandardList(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmRegulatedStandardController sut,
            ConfirmNewRegulatedStandardSubmitModel submitModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var response = await sut.SubmitConfirmationOfRegulatedStandard(submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.ReviewYourDetails);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetProviderAvailableCourseDetailsQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Post_ModelStateIsInvalid_ReturnsViewResult(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmRegulatedStandardController sut,
            ConfirmNewRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            sut.ModelState.AddModelError("key", "message");

            var response = await sut.SubmitConfirmationOfRegulatedStandard(submitModel);

            var result = (ViewResult)response;
            result.Should().NotBeNull();
            result.ViewName.Should().Be(ConfirmRegulatedStandardController.ViewPath);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetProviderAvailableCourseDetailsQuery>(), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public async Task Post_IsNotCorrectStandard_RedirectsToSelectStandard(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IValidator<ConfirmNewRegulatedStandardSubmitModel>> validator,
            [Greedy] ConfirmRegulatedStandardController sut,
            ConfirmNewRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel)
        {
            submitModel.IsApprovedByRegulator = false;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            validator.Setup(x => x.Validate(It.IsAny<ConfirmNewRegulatedStandardSubmitModel>())).Returns(new ValidationResult());

            var response = await sut.SubmitConfirmationOfRegulatedStandard(submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetNeedApprovalToDeliverRegulatedStandard);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetProviderAvailableCourseDetailsQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Post_StandardConfirmed_UpdatesSessionModel(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IValidator<ConfirmNewRegulatedStandardSubmitModel>> validator,
            [Greedy] ConfirmRegulatedStandardController sut,
            ConfirmNewRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel,
            GetProviderAvailableCourseDetailsQueryResult getStandardInformationQueryResult)
        {
            mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderAvailableCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == sessionModel.LarsCode && q.CourseType == CourseType.Apprenticeship),
                    It.IsAny<CancellationToken>())).ReturnsAsync(getStandardInformationQueryResult);
            submitModel.IsApprovedByRegulator = true;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            validator.Setup(x => x.Validate(It.IsAny<ConfirmNewRegulatedStandardSubmitModel>())).Returns(new ValidationResult());

            await sut.SubmitConfirmationOfRegulatedStandard(submitModel);

            sessionServiceMock.Verify(m => m.Set(sessionModel), Times.Once);
            sessionModel.IsConfirmed.Should().BeTrue();
            sessionModel.StandardInformation.Should().BeEquivalentTo(getStandardInformationQueryResult, option => option
                .Excluding(c => c.IsRegulatedForProvider)
                .WithMapping<StandardInformationViewModel>(c => c.Title, v => v.CourseName)
                .WithMapping<StandardInformationViewModel>(c => c.ApprovalBody, v => v.RegulatorName)
                .WithMapping<StandardInformationViewModel>(c => c.Sector, v => v.Sector));

        }

        [Test, MoqAutoData]
        public async Task Post_StandardConfirmed_HasContactDetails_RedirectsUseSavedContactDetails(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IValidator<ConfirmNewRegulatedStandardSubmitModel>> validator,
            [Greedy] ConfirmRegulatedStandardController sut,
            ConfirmNewRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel,
            ProviderContactModel latestProviderContactModel)
        {
            submitModel.IsApprovedByRegulator = true;
            sut.AddDefaultContextWithUser();
            sessionModel.LatestProviderContactModel = latestProviderContactModel;
            sessionModel.IsUsingSavedContactDetails = null;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            validator.Setup(x => x.Validate(It.IsAny<ConfirmNewRegulatedStandardSubmitModel>())).Returns(new ValidationResult());

            var response = await sut.SubmitConfirmationOfRegulatedStandard(submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetAddStandardUseSavedContactDetails);
        }

        [Test, MoqAutoData]
        public async Task Post_StandardConfirmed_RedirectsToContactDetails(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IValidator<ConfirmNewRegulatedStandardSubmitModel>> validator,
            [Greedy] ConfirmRegulatedStandardController sut,
            ConfirmNewRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel)
        {
            submitModel.IsApprovedByRegulator = true;
            sessionModel.LatestProviderContactModel = null;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            validator.Setup(x => x.Validate(It.IsAny<ConfirmNewRegulatedStandardSubmitModel>())).Returns(new ValidationResult());

            var response = await sut.SubmitConfirmationOfRegulatedStandard(submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetAddStandardAddContactDetails);
        }
    }
}
