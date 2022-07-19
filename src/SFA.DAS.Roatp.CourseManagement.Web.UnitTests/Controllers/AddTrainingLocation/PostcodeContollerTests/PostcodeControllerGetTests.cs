using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddTrainingLocation.PostcodeContollerTests
{
    [TestFixture]
    public class PostcodeControllerGetTests
    {
        private readonly string _providerLocationUrl = Guid.NewGuid().ToString();
        private Mock<ISessionService> _sessionServiceMock;
        private PostcodeController _sut;

        [SetUp]
        public void Before_Each_Tests()
        {
            _sessionServiceMock = new Mock<ISessionService>();
            _sut = new PostcodeController(_sessionServiceMock.Object);
            _sut
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderLocations, _providerLocationUrl);
        }

        [Test]
        public void Get_DeletesPostcodeFromSession()
        {
            _sut.GetPostcode();

            _sessionServiceMock.Verify(s => s.Delete(SessionKeys.SelectedPostcode, TestConstants.DefaultUkprn));
        }

        [Test]
        public void Get_ReturnsViewResult()
        {
            var result = (ViewResult)_sut.GetPostcode();

            result.Should().NotBeNull();
            var model = (PostcodeViewModel)result.Model;
            model.BackLink.Should().Be(_providerLocationUrl);
            model.CancelLink.Should().Be(_providerLocationUrl);
        }
    }
}