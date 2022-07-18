using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations.AddProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddTrainingLocationControllerTests
{
    [TestFixture]
    public class AddTrainingLocationControllerGetTests
    {
        AddTrainingLocationController Sut;
        Mock<ISessionService> _sessionServiceMock;
        readonly string _providerLocationUrl = Guid.NewGuid().ToString();

        [SetUp]
        public void Before_Each_Tests()
        {
            _sessionServiceMock = new Mock<ISessionService>();
            Sut = new AddTrainingLocationController(_sessionServiceMock.Object, Mock.Of<ILogger<AddTrainingLocationController>>());
            Sut
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderLocations, _providerLocationUrl);
        }

        [Test]
        public void Get_ClearsPostcodeFromSession()
        {
            Sut.Postcode();

            _sessionServiceMock.Verify(s => s.Delete(SessionKeys.SelectedPostcode, TestConstants.DefaultUkprn));
        }

        [Test]
        public void Get_ReturnsViewResult()
        {
            var result = (ViewResult) Sut.Postcode();

            _sessionServiceMock.Verify(s => s.Delete(SessionKeys.SelectedPostcode, TestConstants.DefaultUkprn));
            result.Should().NotBeNull();
            var model = (ProviderLocationPostcodeViewModel)result.Model;
            model.BackLink.Should().Be(_providerLocationUrl);
            model.CancelLink.Should().Be(_providerLocationUrl);
        }
    }
}