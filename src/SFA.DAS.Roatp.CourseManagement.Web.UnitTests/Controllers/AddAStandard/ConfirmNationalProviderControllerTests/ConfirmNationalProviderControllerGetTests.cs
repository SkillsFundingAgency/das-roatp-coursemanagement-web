using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.ConfirmNationalProviderControllerTests
{
    [TestFixture]
    public class ConfirmNationalProviderControllerGetTests
    {
        [Test, MoqAutoData]
        public void Get_ModelMissingFromSession_RedirectsToReviewYourDetails(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNationalProviderController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var result = sut.ConfirmNationalDeliveryOption();

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ReviewYourDetails);
        }

        [Test, MoqAutoData]
        public void Get_LocationOptionIsProviderLocation_RedirectsToManageStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNationalProviderController sut,
            StandardSessionModel sessionModel)
        {
            sessionModel.LocationOption = LocationOption.ProviderLocation;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            sut.AddDefaultContextWithUser();

            var result = sut.ConfirmNationalDeliveryOption();

            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ViewStandards);
        }

        [Test, MoqAutoData]
        public void Get_LocationOptionIsEmployer_ReturnViewResult(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] ConfirmNationalProviderController sut,
            StandardSessionModel sessionModel)
        {
            sessionModel.LocationOption = LocationOption.EmployerLocation;
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
            sut.AddDefaultContextWithUser();

            var result = sut.ConfirmNationalDeliveryOption();

            result.As<ViewResult>().ViewName.Should().Be(ConfirmNationalProviderController.ViewPath);
        }
    }
}
