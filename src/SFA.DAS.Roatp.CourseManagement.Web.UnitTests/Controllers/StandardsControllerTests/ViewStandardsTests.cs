using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.StandardsControllerTests
{
    [TestFixture]
    public class ViewStandardsTests
    {
        private StandardsController _controller;
        private Mock<ILogger<StandardsController>> _logger;
        private Mock<IMediator> _mediator;
        private Mock<IUrlHelper> urlHelper;
        string verifyUrl = "http://test";
        string verifyStandardUrl = "http://test-standard";

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<StandardsController>>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111"), }, "mock"));

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
            _mediator.Setup(x => x.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetAllProviderStandardsQueryResult
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
            urlHelper = new Mock<IUrlHelper>();

            UrlRouteContext verifyRouteValues2 = null;
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c=>c.RouteName.Equals(RouteNames.ViewStandardDetails))
                ))
                .Returns(verifyStandardUrl)
                .Callback<UrlRouteContext>(c =>
                {
                    verifyRouteValues2 = c;
                });

            UrlRouteContext verifyRouteValues = null;
            urlHelper
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                   c.RouteName.Equals(RouteNames.ReviewYourDetails)
               )))
               .Returns(verifyUrl)
               .Callback<UrlRouteContext>(c =>
               {
                   verifyRouteValues = c;
               });

            _controller.Url = urlHelper.Object;
        }

        [Test]
        public async Task StandardsController_ViewStandards_ReturnsValidResponse()
        {
            var result = await _controller.ViewStandards();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewStandards.cshtml");
            viewResult.Model.Should().NotBeNull();
            var model = viewResult.Model as StandardListViewModel;
            model.Should().NotBeNull();
            model.Standards.Should().NotBeNull();
            model.BackUrl.Should().Be(verifyUrl);
            model.Standards.First().StandardUrl.Should().Be(verifyStandardUrl);
        }

        [Test]
        public async Task StandardsController_ViewStandards_ReturnsNoStandardData()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(() => null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{ new Claim(ProviderClaims.ProviderUkprn,"111"),}, "mock"));
            
            _controller = new StandardsController(_mediator.Object, _logger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
                TempData = Mock.Of<ITempDataDictionary>()
            };
            _controller.Url = urlHelper.Object;
            var result = await _controller.ViewStandards();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewStandards.cshtml");
            viewResult.Model.Should().NotBeNull();
            var model = viewResult.Model as StandardListViewModel;
            model.Should().NotBeNull();
            model.Standards.Should().BeEmpty();
            model.BackUrl.Should().Be(verifyUrl);
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Exactly(2));
        }
    }
}
