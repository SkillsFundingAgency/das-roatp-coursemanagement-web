using AutoFixture.NUnit3;
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
    public class AddTrainingLocationControllerPostTests
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
        public void Post_ModelStateIsInvalid_ReturnsViewResult()
        {
            Sut.ModelState.AddModelError("errorKey", "errorMessage");

            var result = (ViewResult) Sut.Postcode(new ProviderLocationPostcodeViewModel());

            _sessionServiceMock.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            result.Should().NotBeNull();
            var model = (ProviderLocationPostcodeViewModel)result.Model;
            model.BackLink.Should().Be(_providerLocationUrl);
            model.CancelLink.Should().Be(_providerLocationUrl);
        }
        
        [Test, AutoData]
        public void Post_ModelStateIsValid_PersistsPostcodeInSesssion(ProviderLocationPostcodeViewModel model)
        {
            Sut.Postcode(model);
            _sessionServiceMock.Verify(s => s.Set(model.Postcode, SessionKeys.SelectedPostcode, TestConstants.DefaultUkprn));
        }

        [Test, AutoData]
        public void Post_ModelStateIsValid_RedirectsToGetTrainingLocationAddress(ProviderLocationPostcodeViewModel model)
        {
            var result = (RedirectToRouteResult) Sut.Postcode(model);

            result.RouteName.Should().Be(RouteNames.GetTrainingLocationAddress);
        }
    }
}