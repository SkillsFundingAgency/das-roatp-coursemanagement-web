using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
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
        public void Get_ModelMissingFromSession_RedirectsToSelectAStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddContactDetailsController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns((StandardSessionModel)null);

            var result = sut.SubmitContactDetails(new EditCourseContactDetailsSubmitModel());

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
        }


        [Test, MoqAutoData]
        public void Get_ModelStateIsInvalid_ReturnsViewResult(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddContactDetailsController sut,
             StandardSessionModel standardSessionModel,
             string cancelLink)
        {
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.ViewStandards, cancelLink);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(standardSessionModel);
            sut.ModelState.AddModelError("key", "message");

            var result = sut.SubmitContactDetails(new EditCourseContactDetailsSubmitModel());

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(AddContactDetailsController.ViewPath);
            result.As<ViewResult>().Model.As<AddStandardContactDetailsViewModel>().CancelLink.Should().Be(cancelLink);
        }

        [Test, MoqAutoData]
        public void Get_ModelStateIsValid_UpdatedStandardSessionModel(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddContactDetailsController sut,
            StandardSessionModel standardSessionModel,
            EditCourseContactDetailsSubmitModel submitModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(standardSessionModel);

            sut.SubmitContactDetails(submitModel);

            sessionServiceMock.Verify(s => s.Set(
                It.Is<StandardSessionModel>(m => 
                    m.ContactUsPhoneNumber == submitModel.ContactUsPhoneNumber &&
                    m.ContactUsPageUrl == submitModel.ContactUsPageUrl &&
                    m.ContactUsEmail == submitModel.ContactUsEmail &&
                    m.StandardInfoUrl == submitModel.StandardInfoUrl
                ), 
                TestConstants.DefaultUkprn));
        }

    }
}
