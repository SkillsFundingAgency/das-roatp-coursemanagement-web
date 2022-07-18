using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseLocationsControllerTests
{
    [TestFixture]
    public class ProviderCourseLocationsControllerGetTests
    {
        private const string Ukprn = "10012002";
        private Mock<ILogger<ProviderCourseLocationsController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private ProviderCourseLocationsController _sut;
        private Mock<IUrlHelper> urlHelper;
        string verifyUrl = "http://test";

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<ProviderCourseLocationsController>>();
            _mediatorMock = new Mock<IMediator>();
           
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), }, "mock"));
            var httpContext = new DefaultHttpContext() { User = user };
            _sut = new ProviderCourseLocationsController(_mediatorMock.Object, _loggerMock.Object)
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
                   c.RouteName.Equals(RouteNames.GetStandardDetails)
               )))
               .Returns(verifyUrl)
               .Callback<UrlRouteContext>(c =>
               {
                   verifyRouteValues = c;
               });
            _sut.Url = urlHelper.Object;
        }

        [Test, AutoData]
        public async Task Get_ValidRequest_ReturnsView(
            GetProviderCourseLocationsQueryResult queryResult,
            int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderCourseLocations(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ProviderCourseLocationListViewModel;
            model.Should().NotBeNull();
            model.BackUrl.Should().NotBeNull();
            model.CancelUrl.Should().NotBeNull();
        }

          [Test, AutoData]
        public async Task Get_InvalidRequest_ThrowsInvalidOperationException(int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetProviderCourseLocationsQueryResult)null);

            Func<Task> action = () => _sut.GetProviderCourseLocations(larsCode);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
