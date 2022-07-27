using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ViewProviderLocationDetailsControllerTests
{
    [TestFixture]
    public class ViewProviderLocationDetailsControllerTests
    {
        private const string Ukprn = "10012002";
        private Mock<ILogger<ViewProviderLocationDetailsController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private ViewProviderLocationDetailsController _sut;
        private Mock<IUrlHelper> urlHelper;
        string verifyUrl = "http://test";

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<ViewProviderLocationDetailsController>>();
            _mediatorMock = new Mock<IMediator>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), }, "mock"));
            var httpContext = new DefaultHttpContext() { User = user };
            _sut = new ViewProviderLocationDetailsController(_mediatorMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            urlHelper = new Mock<IUrlHelper>();

            UrlRouteContext verifyRouteValues = null;
            urlHelper
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                   c.RouteName.Equals(RouteNames.ViewProviderLocations)
               )))
               .Returns(verifyUrl)
               .Callback<UrlRouteContext>(c =>
               {
                   verifyRouteValues = c;
               });
            _sut.Url = urlHelper.Object;
        }

        [Test, AutoData]
        public async Task GetProviderLocationDetails_ValidRequest_ReturnsView(
            GetProviderLocationDetailsQueryResult queryResult,
            Guid id)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderLocationDetails(id);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewProviderLocationsDetails.cshtml");
            var model = viewResult.Model as ProviderLocationViewModel;
            model.Should().NotBeNull();
            model.BackUrl.Should().NotBeNull();
        }

          [Test, AutoData]
        public async Task GetProviderLocationDetails_InvalidRequest_ReturnsEmptyResponse(Guid id)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderLocationDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetProviderLocationDetailsQueryResult)null);

            var result = await _sut.GetProviderLocationDetails(id);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewProviderLocationsDetails.cshtml");
            var model = viewResult.Model as ProviderLocationViewModel;
            model.Should().NotBeNull();
            model.BackUrl.Should().NotBeNull();
        }
    }
}
