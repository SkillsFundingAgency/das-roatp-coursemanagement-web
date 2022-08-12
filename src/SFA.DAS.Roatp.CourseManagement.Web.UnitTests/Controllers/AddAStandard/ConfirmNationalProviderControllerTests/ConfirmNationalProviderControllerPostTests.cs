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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.ConfirmNationalProviderControllerTests
{
    [TestFixture]
    public class ConfirmNationalProviderControllerPostTests
    {
        [Test, MoqAutoData]
        public void Post_ModelMissingFromSession_RedirectsToSelectAStandard(
           [Frozen] Mock<ISessionService> sessionServiceMock,
           [Greedy] ConfirmNationalProviderController sut,
           ConfirmNationalProviderSubmitModel submitModel)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns((StandardSessionModel)null);

            var result = sut.SubmitConfirmationOnNationalProvider(submitModel);

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
        }

        [Test, MoqAutoData]
        public void Post_ModelStateIsInvalid_ReturnsViewResult(
           [Frozen] Mock<ISessionService> sessionServiceMock,
           [Greedy] ConfirmNationalProviderController sut,
           ConfirmNationalProviderSubmitModel submitModel,
           StandardSessionModel sessionModel,
           string cancelLink)
        {
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.ViewStandards, cancelLink);
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(sessionModel);
            sut.ModelState.AddModelError("key", "message");

            var result = sut.SubmitConfirmationOnNationalProvider(submitModel);

            result.As<ViewResult>().ViewName.Should().Be(ConfirmNationalProviderController.ViewPath);
            result.As<ViewResult>().Model.As<ConfirmNationalProviderViewModel>().CancelLink.Should().Be(cancelLink);
        }

        [Test, MoqAutoData]
        public void Post_ModelStateIsValid_UpdatesSessionModel(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNationalProviderController sut,
            StandardSessionModel sessionModel,
            bool hasNationalDeliveryOption)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>(It.IsAny<string>())).Returns(sessionModel);
            sut.AddDefaultContextWithUser();

            sut.SubmitConfirmationOnNationalProvider(new ConfirmNationalProviderSubmitModel { HasNationalDeliveryOption = hasNationalDeliveryOption });

            sessionServiceMock.Verify(s => s.Set(It.Is<StandardSessionModel>(m => m.HasNationalDeliveryOption == hasNationalDeliveryOption), It.IsAny<string>()));
        }
    }
}
