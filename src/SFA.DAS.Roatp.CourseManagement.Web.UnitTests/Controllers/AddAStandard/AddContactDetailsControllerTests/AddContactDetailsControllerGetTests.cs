using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.AddContactDetailsControllerTests
{
    [TestFixture]
    public class AddContactDetailsControllerGetTests
    {
        [Test, MoqAutoData]
        public void Get_ModelMissingFromSession_RedirectsToReviewYourDetails(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddContactDetailsController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var result = sut.GetContactDetails();

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ReviewYourDetails);
        }

        [Test, MoqAutoData]
        public void Get_StandardSessionModelIsSet_RedirectsToReviewYourDetails(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddContactDetailsController sut,
            StandardSessionModel standardSessionModel)
        {
            standardSessionModel.LatestProviderContactModel = null;
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);

            var result = sut.GetContactDetails();

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(AddContactDetailsController.ViewPath);

            var viewResult = result as ViewResult;
            var model = viewResult!.Model as AddStandardContactDetailsViewModel;
            model!.ContactUsEmail.Should().Be(standardSessionModel.ContactInformation.ContactUsEmail);
            model.ContactUsPhoneNumber.Should().Be(standardSessionModel.ContactInformation.ContactUsPhoneNumber);
            model.StandardInfoUrl.Should().Be(standardSessionModel.ContactInformation.StandardInfoUrl);
        }

        [Test]
        [MoqInlineAutoData(null, null, null, false)]
        [MoqInlineAutoData(true, "test@test.com", "1234", true)]
        [MoqInlineAutoData(true, null, "1234", true)]
        [MoqInlineAutoData(true, "test@test.com", null, true)]
        [MoqInlineAutoData(false, "test@test.com", "1234", false)]
        public void Get_SetsUpEmailPhone_RedirectsToSelectAStandard(
            bool? isUsingSavedContactDetails,
            string contactUsEmail,
            string contactPhoneNumber,
            bool showSavedContactDetailsText,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddContactDetailsController sut,
            StandardSessionModel standardSessionModel)
        {
            standardSessionModel.ContactInformation = new StandardContactInformationViewModel { ContactUsEmail = null, ContactUsPhoneNumber = null };
            standardSessionModel.IsUsingSavedContactDetails = isUsingSavedContactDetails;
            standardSessionModel.LatestProviderContactModel = null;
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

            var result = sut.GetContactDetails();
            var viewResult = result as ViewResult;
            var model = viewResult!.Model as AddStandardContactDetailsViewModel;
            model!.ShowSavedContactDetailsText.Should().Be(showSavedContactDetailsText);
            model.ContactUsEmail.Should().Be(contactUsEmail);
            model.ContactUsPhoneNumber.Should().Be(contactPhoneNumber);
            model.StandardInfoUrl.Should().BeNull();
        }
    }
}
