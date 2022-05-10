using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class GetProvidersTrainingLocationsTest
    {
        private ProviderLocationsController _controller;
        private Mock<ILogger<ProviderLocationsController>> _logger;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<ProviderLocationsController>>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111"), }, "mock"));

            var response = new System.Collections.Generic.List<ProviderLocation>();

            var ProviderLocation1 = new ProviderLocation
            {
                LocationId = 1,
                LocationName = "test1",
                Postcode = "IG117WQ",
                Email = "test1@test.com",
                Phone="1234567891"
            };
            var ProviderLocation2 = new ProviderLocation
            {
                LocationId = 2,
                LocationName = "test2",
                Postcode = "IG117XR",
                Email = "test2@test.com",
                Phone = "1234567892"
            };
            response.Add(ProviderLocation1);
            response.Add(ProviderLocation2);

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetProviderLocationQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetProviderLocationQueryResult
                {
                    ProviderLocations = response
                });

            _controller = new ProviderLocationsController(_mediator.Object,  _logger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
                TempData = Mock.Of<ITempDataDictionary>()
            };
        }

        [Test]
        public async Task GetProvidersTrainingLocation_ReturnsValidResponse()
        {
            var result = await _controller.GetProvidersTrainingLocation();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewProviderLocations.cshtml");
            var model = viewResult.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();
            model.ProviderLocations.Should().NotBeNull();
        }

        [Test]
        public async Task GetProvidersTrainingLocation_ReturnsNoProviderLocationData()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetProviderLocationQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(() => null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{ new Claim(ProviderClaims.ProviderUkprn,"111"),}, "mock"));
            
            _controller = new ProviderLocationsController(_mediator.Object, _logger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
                TempData = Mock.Of<ITempDataDictionary>()
            };
            var result = await _controller.GetProvidersTrainingLocation();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewProviderLocations.cshtml");
            var model = viewResult.Model as ProviderLocationListViewModel;
            model.Should().NotBeNull();
            model.ProviderLocations.Should().BeEmpty();
        }
    }
}
