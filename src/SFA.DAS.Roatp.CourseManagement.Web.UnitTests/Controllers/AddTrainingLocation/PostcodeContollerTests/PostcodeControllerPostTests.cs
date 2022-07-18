using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddTrainingLocation.PostcodeContollerTests
{
    [TestFixture]
    public class PostcodeControllerPostTests
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
        public void Post_ModelStateIsInvalid_ReturnsViewResult()
        {
            Sut.ModelState.AddModelError("errorKey", "errorMessage");

            var result = (ViewResult)Sut.SubmitPostcode(new PostcodeViewModel());

            result.Should().NotBeNull();
            var model = (PostcodeViewModel)result.Model;
            model.BackLink.Should().Be(_providerLocationUrl);
            model.CancelLink.Should().Be(_providerLocationUrl);
        }

        [Test, AutoData]
        public void Post_ModelStateIsValid_RedirectsToGetTrainingLocationAddress(PostcodeSubmitModel model)
        {
            var result = (RedirectToRouteResult)Sut.SubmitPostcode(model);

            result.RouteName.Should().Be(RouteNames.GetTrainingLocationAddress);
            result.RouteValues.Should().HaveCount(2);
            result.RouteValues[nameof(model.Postcode)].Should().Be(model.Postcode);
        }
    }
}