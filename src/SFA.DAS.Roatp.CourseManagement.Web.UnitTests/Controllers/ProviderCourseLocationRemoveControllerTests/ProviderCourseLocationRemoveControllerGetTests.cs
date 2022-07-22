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
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseLocationRemoveControllerTests
{
    [TestFixture]
    public class ProviderCourseLocationRemoveControllerGetTests
    {
        private const string Ukprn = "10012002";
        private Mock<ILogger<ProviderCourseLocationRemoveController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private ProviderCourseLocationRemoveController _sut;
        private Mock<IUrlHelper> urlHelper;
        string verifyUrl = "http://test";

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<ProviderCourseLocationRemoveController>>();
            _mediatorMock = new Mock<IMediator>();
           
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), }, "mock"));
            var httpContext = new DefaultHttpContext() { User = user };
            _sut = new ProviderCourseLocationRemoveController(_mediatorMock.Object, _loggerMock.Object)
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
                   c.RouteName.Equals(RouteNames.GetProviderCourseLocations)
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
            int id = queryResult.ProviderCourseLocations.Where(a => a.Id > 0).FirstOrDefault().Id;
            var result = await _sut.GetProviderCourseLocation(larsCode, id);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ProviderCourseLocationViewModel;
            model.Should().NotBeNull();
            model.BackLink.Should().NotBeNull();
            model.CancelLink.Should().NotBeNull();
        }

          [Test, AutoData]
        public async Task Get_InvalidRequest_ThrowsInvalidOperationException(int larsCode, int id)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetProviderCourseLocationsQueryResult)null);

            Func<Task> action = () => _sut.GetProviderCourseLocation(larsCode, id);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
