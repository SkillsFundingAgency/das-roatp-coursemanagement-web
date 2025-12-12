using System.Collections.Generic;
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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.SelectLocationOptionControllerTests
{
    [TestFixture]
    public class SelectLocationOptionControllerGetTests
    {
        [Test, MoqAutoData]
        public void SelectLocationOption_SessionNotAvailable_RedirectsToSelectStandard(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] SelectLocationOptionController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

            var result = sut.SelectLocationOption();

            result.As<RedirectToRouteResult>().Should().NotBeNull();
            result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ReviewYourDetails);
        }

        [Test, MoqAutoData]
        public void SelectLocationOption_ReturnsView(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] SelectLocationOptionController sut)
        {
            sut.AddDefaultContextWithUser().AddUrlHelperMock();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(new StandardSessionModel { LocationOption = LocationOption.ProviderLocation, LarsCode = 1 });

            var result = sut.SelectLocationOption();

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(SelectLocationOptionController.ViewPath);
            result.As<ViewResult>().Model.As<SelectLocationOptionViewModel>().Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public void SelectLocationOption_ResetCourseLocations(
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] SelectLocationOptionController sut,
            string larsCode)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(new StandardSessionModel { LocationOption = LocationOption.ProviderLocation, LarsCode = larsCode, CourseLocations = new List<CourseLocationModel> { new CourseLocationModel() } });

            sut.SelectLocationOption();

            sessionServiceMock.Verify(m =>
                m.Set(It.Is<StandardSessionModel>(x => x.LocationOption == LocationOption.ProviderLocation && x.LarsCode == larsCode && x.CourseLocations.Count == 0)
                ), Times.Once);
        }
    }
}
