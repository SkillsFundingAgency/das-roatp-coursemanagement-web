using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.ApiClients;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class StandardsControllerTests
    {
        private StandardsController _controller;
        private Mock<IRoatpCourseManagementOuterApiClient> _outerApiClient;
        private Mock<ILogger<StandardsController>> _logger;
        private int _ukprn;
        private StandardsListViewModel expectedModel;

        [SetUp]
        public void Before_each_test()
        {
            _outerApiClient = new Mock<IRoatpCourseManagementOuterApiClient>();
            _logger = new Mock<ILogger<StandardsController>>();
            _ukprn = 111;

            var signInId = Guid.NewGuid();
            var givenNames = "Test";
            var familyName = "User";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, $"{givenNames} {familyName}"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("Email", "test@test.com"),
                new Claim("sub", signInId.ToString()),
                new Claim("custom-claim", "example claim value"),
                new Claim(ProviderClaims.ProviderUkprn,"111"),
            }, "mock"));

            var response = new StandardsListViewModel
            {
                Standards = new System.Collections.Generic.List<StandardsViewModel>()
            };

            var standard1 = new StandardsViewModel
            {
                ProviderCourseId = 1,
                CourseName = "test1",
                Level =1,
                IsImported = true
            };
            var standard2 = new StandardsViewModel
            {
                ProviderCourseId = 2,
                CourseName = "test2",
                Level = 2,
                IsImported = false
            };
            response.Standards.Add(standard1);
            response.Standards.Add(standard2);
            expectedModel = response;

            _outerApiClient.Setup(x => x.GetAllStandards(_ukprn)).ReturnsAsync(response);

            _controller = new StandardsController(_outerApiClient.Object,  _logger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
                TempData = Mock.Of<ITempDataDictionary>()
            };
        }

        [Test]
        public async Task StandardsController_GetStandards_ReturnsValidResponse()
        {
            var result = await _controller.GetStandards();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewStandards.cshtml");
            viewResult.Model.Should().NotBeNull();
            viewResult.Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
