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
using SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
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
            ConfirmNonRegulatedStandardSubmitModel submitModel,
            int ukprn)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var response = await sut.SubmitConfirmationOfStandard(ukprn, submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.ReviewYourDetails);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetProviderAvailableCourseDetailsQuery>(), It.IsAny<CancellationToken>()), Times.Never);
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
            mediatorMock.Verify(m => m.Send(It.IsAny<GetProviderAvailableCourseDetailsQuery>(), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public async Task Post_IsNotCorrectStandard_RedirectsToSelectStandard(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IValidator<ConfirmNonRegulatedStandardSubmitModel>> validator,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            ConfirmNonRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel,
            int ukprn)
        {
            submitModel.IsCorrectStandard = false;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            validator.Setup(x => x.Validate(It.IsAny<ConfirmNonRegulatedStandardSubmitModel>())).Returns(new ValidationResult());

            var response = await sut.SubmitConfirmationOfStandard(ukprn, submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetProviderAvailableCourseDetailsQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Post_StandardConfirmed_UpdatesSessionModel(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IValidator<ConfirmNonRegulatedStandardSubmitModel>> validator,
            [Greedy] ConfirmNonRegulatedStandardController sut,
            ConfirmNonRegulatedStandardSubmitModel submitModel,
            StandardSessionModel sessionModel,
            GetProviderAvailableCourseDetailsQueryResult getStandardInformationQueryResult)
        {
            var ukprn = 123456789;

            mediatorMock.Setup(m => m.Send(It.Is<GetProviderAvailableCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == sessionModel.LarsCode && q.CourseType == CourseType.Apprenticeship), It.IsAny<CancellationToken>())).ReturnsAsync(getStandardInformationQueryResult);
            submitModel.IsCorrectStandard = true;
            sut.AddDefaultContextWithUser();
            sessionModel.LatestProviderContactModel = null;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            mediatorMock.Setup(m => m.Send(It.IsAny<GetLatestProviderContactQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetLatestProviderContactQueryResult)null);
            validator.Setup(x => x.Validate(It.IsAny<ConfirmNonRegulatedStandardSubmitModel>())).Returns(new ValidationResult());

            await sut.SubmitConfirmationOfStandard(ukprn, submitModel);

            sessionServiceMock.Verify(m => m.Set(sessionModel), Times.Once);
            sessionModel.IsConfirmed.Should().BeTrue();
            sessionModel.StandardInformation.Should().BeEquivalentTo(getStandardInformationQueryResult, option => option
                .WithMapping<StandardInformationViewModel>(c => c.LarsCode, v => v.LarsCode)
                .WithMapping<StandardInformationViewModel>(c => c.Level, v => v.Level)
                .WithMapping<StandardInformationViewModel>(c => c.Title, v => v.CourseName)
                .WithMapping<StandardInformationViewModel>(c => c.ApprovalBody, v => v.RegulatorName)
                .WithMapping<StandardInformationViewModel>(c => c.Sector, v => v.Sector)
                .WithMapping<StandardInformationViewModel>(c => c.LastDateStarts, v => v.LastDateStarts)
                .WithMapping<StandardInformationViewModel>(c => c.IfateReferenceNumber, v => v.IfateReferenceNumber)
                .WithMapping<StandardInformationViewModel>(c => c.LearningType, v => v.LearningType));
        }

        [Test, MoqAutoData]
        public async Task Post_StandardConfirmed_RedirectsToContactDetails(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Frozen] Mock<IValidator<ConfirmNonRegulatedStandardSubmitModel>> validator,
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
            validator.Setup(x => x.Validate(It.IsAny<ConfirmNonRegulatedStandardSubmitModel>())).Returns(new ValidationResult());

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
            [Frozen] Mock<IValidator<ConfirmNonRegulatedStandardSubmitModel>> validator,
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
            validator.Setup(x => x.Validate(It.IsAny<ConfirmNonRegulatedStandardSubmitModel>())).Returns(new ValidationResult());

            var response = await sut.SubmitConfirmationOfStandard(ukprn, submitModel);

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetAddStandardUseSavedContactDetails);
        }
    }
}
