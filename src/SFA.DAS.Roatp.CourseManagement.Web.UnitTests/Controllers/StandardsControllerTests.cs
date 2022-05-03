using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Roatp.CourseManagement.Domain.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class StandardsControllerTests
    {
        private StandardsController _controller;
        private Mock<ILogger<StandardsController>> _logger;
        private Mock<IMediator> _mediator;
        private StandardListViewModel expectedModel;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<StandardsController>>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ProviderClaims.ProviderUkprn,"111"),
            }, "mock"));

            var response = new System.Collections.Generic.List<Standard>();

            var standard1 = new Standard
            {
                ProviderCourseId = 1,
                CourseName = "test1",
                Level =1,
                IsImported = true
            };
            var standard2 = new Standard
            {
                ProviderCourseId = 2,
                CourseName = "test2",
                Level = 2,
                IsImported = false
            };
            response.Add(standard1);
            response.Add(standard2);

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetStandardQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetStandardQueryResult
                {
                    Standards = response
                });

            _controller = new StandardsController(_mediator.Object,  _logger.Object)
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
            var result = await _controller.ViewStandards();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewStandards.cshtml");
            viewResult.Model.Should().NotBeNull();
        }
    }
}
