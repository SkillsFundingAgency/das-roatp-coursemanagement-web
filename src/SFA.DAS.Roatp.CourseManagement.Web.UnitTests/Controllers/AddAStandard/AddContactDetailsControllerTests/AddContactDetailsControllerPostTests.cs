using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.AddContactDetailsControllerTests
{
    [TestFixture]
    public class AddContactDetailsControllerPostTests
    {
        [Test, MoqAutoData]
        public void Get_ModelMissingFromSession_RedirectsToReviewYourDetails(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddContactDetailsController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var result = sut.SubmitContactDetails(new CourseContactDetailsSubmitModel());

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ReviewYourDetails);
        }


        [Test, MoqAutoData]
        public void Get_ModelStateIsInvalid_ReturnsViewResult(
            bool? isUsingSavedContactDetails,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddContactDetailsController sut,
             StandardSessionModel standardSessionModel,
            string contactUsEmail,
            string contactPhoneNumber)
        {
            var showSavedContactDetailsText = isUsingSavedContactDetails is true;

            if (!showSavedContactDetailsText)
            {
                contactUsEmail = null;
                contactPhoneNumber = null;
            }

            if (isUsingSavedContactDetails.HasValue)
            {
                standardSessionModel.LatestProviderContactModel = new ProviderContactModel
                {
                    EmailAddress = contactUsEmail,
                    PhoneNumber = contactPhoneNumber
                };
            }

            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
            sut.ModelState.AddModelError("key", "message");

            var result = sut.SubmitContactDetails(new CourseContactDetailsSubmitModel());

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(AddContactDetailsController.ViewPath);
            result.As<ViewResult>().Model.As<AddStandardContactDetailsViewModel>().ShowSavedContactDetailsText.Should().Be(showSavedContactDetailsText);
        }

        [Test, MoqAutoData]
        public void Get_ModelStateIsValid_UpdatesStandardSessionModel(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddContactDetailsController sut,
            StandardSessionModel standardSessionModel,
            CourseContactDetailsSubmitModel submitModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);

            sut.SubmitContactDetails(submitModel);

            sessionServiceMock.Verify(s => s.Set(standardSessionModel));

            standardSessionModel.ContactInformation.ContactUsPhoneNumber.Should().Be(submitModel.ContactUsPhoneNumber);
            standardSessionModel.ContactInformation.ContactUsEmail.Should().Be(submitModel.ContactUsEmail);
            standardSessionModel.ContactInformation.StandardInfoUrl.Should().Be(submitModel.StandardInfoUrl);
        }

        [Test, MoqAutoData]
        public void Get_ModelStateIsValid_RedirectToSelectLocationOption(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddContactDetailsController sut,
            StandardSessionModel standardSessionModel,
            CourseContactDetailsSubmitModel submitModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);

            var result = sut.SubmitContactDetails(submitModel);

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectLocationOption);
        }
    }
}
