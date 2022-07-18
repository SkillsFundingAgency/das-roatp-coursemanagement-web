using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        PostcodeController Sut;
        readonly string _providerLocationUrl = Guid.NewGuid().ToString();

        [SetUp]
        public void Before_Each_Tests()
        {
            Sut = new PostcodeController();
            Sut
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderLocations, _providerLocationUrl);
        }

        [Test]
        public void Get_ReturnsViewResult()
        {
            var result = (ViewResult)Sut.GetPostcode();

            result.Should().NotBeNull();
            var model = (PostcodeViewModel)result.Model;
            model.BackLink.Should().Be(_providerLocationUrl);
            model.CancelLink.Should().Be(_providerLocationUrl);
        }
    }
}