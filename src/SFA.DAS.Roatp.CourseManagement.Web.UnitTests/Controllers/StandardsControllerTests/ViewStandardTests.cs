using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.StandardsControllerTests
{
    [TestFixture]
    public class ViewStandardTests
    {
        private StandardsController _controller;
        private Mock<ILogger<StandardsController>> _logger;
        private Mock<IMediator> _mediator;
        private const int Ukprn = 10000001;
        private const int LarsCode = 123;
        private const string Version = "1.1";
        private Mock<IUrlHelper> urlHelper;
        private const string verifyUrl = "http://test";
        private const string Regulator = "Test-Regulator";

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<StandardsController>>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn.ToString()), }, "mock"));

            var response = new StandardDetails
            {
                CourseName = "test1",
                Level = "1",
                IFateReferenceNumber = "1234",
                Sector = "Digital",
                LarsCode = LarsCode,
                RegulatorName = "",
                Version = Version,
                StandardInfoUrl = "www.test.com",
                ContactUsEmail = "test@test.com",
                ContactUsPageUrl = "www.test.com/ContactUs",
                ContactUsPhoneNumber = "123456789"
            };
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetStandardDetailsQueryResult
                {
                    StandardDetails = response
                });

            _controller = new StandardsController(_mediator.Object, _logger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
                TempData = Mock.Of<ITempDataDictionary>()
            };

            urlHelper = new Mock<IUrlHelper>();

            UrlRouteContext verifyRouteValues = null;
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                    c.RouteName.Equals(RouteNames.ViewStandards)
                )))
                .Returns(verifyUrl)
                .Callback<UrlRouteContext>(c =>
                {
                    verifyRouteValues = c;
                });
            _controller.Url = urlHelper.Object;
        }

        [Test]
        public async Task ViewStandard_ReturnsValidResponse()
        {
            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewStandardDetails.cshtml");
            viewResult.Model.Should().NotBeNull();
            var model = viewResult.Model as StandardDetailsViewModel;
            model.Should().NotBeNull();
            model.LarsCode.Should().Be(LarsCode);
            model.Version.Should().Be(Version);
            model.BackUrl.Should().Be(verifyUrl);
            _logger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Never);
        }

        [Test]
        public async Task ViewStandard_HandlerReturnsNull_ThrowsInvalidOperaionException()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(() => null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn.ToString()), }, "mock"));
        
            _controller = new StandardsController(_mediator.Object, _logger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
                TempData = Mock.Of<ITempDataDictionary>()
            };

            Func<Task> action = () => _controller.ViewStandard(LarsCode);

            await action.Should().ThrowAsync<InvalidOperationException>();
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Test]
        public async Task ViewStandard_ResponseIncludesRegulator()
        {
            var response = new StandardDetails
            {
                RegulatorName = Regulator,
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetStandardDetailsQueryResult
                {
                    StandardDetails = response
                });
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn.ToString()), }, "mock"));

            _controller = new StandardsController(_mediator.Object, _logger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
                TempData = Mock.Of<ITempDataDictionary>()
            };

            urlHelper = new Mock<IUrlHelper>();

            _controller.Url = urlHelper.Object;

            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as StandardDetailsViewModel;
            model.Should().NotBeNull();
            model.RegulatorName.Should().Be(Regulator);
            model.IsStandardRegulated.Should().Be(true);
        }

        [Test]
        public async Task ViewStandard_PopulatesEditContactDetailsUrl()
        {
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                    c.RouteName.Equals(RouteNames.GetCourseContactDetails)
                )))
                .Returns(verifyUrl);
            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as StandardDetailsViewModel;
            model.EditContactDetailsUrl.Should().Be(verifyUrl);
        }

        [Test]
        public async Task ViewStandard_PopulatesEditLocationOptionUrl()
        {
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                    c.RouteName.Equals(RouteNames.GetLocationOption)
                )))
                .Returns(verifyUrl);
            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as StandardDetailsViewModel;
            model.EditLocationOptionUrl.Should().Be(verifyUrl);
        }
    }
}
