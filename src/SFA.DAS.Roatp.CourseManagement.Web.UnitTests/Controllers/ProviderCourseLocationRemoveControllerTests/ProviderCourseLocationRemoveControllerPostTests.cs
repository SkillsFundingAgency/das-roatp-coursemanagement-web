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
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseLocationRemoveControllerTests
{
    [TestFixture]
    public class ProviderCourseLocationRemoveControllerPostTests
    {
        private const string Ukprn = "10012002";
        private static string UserId = Guid.NewGuid().ToString();
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

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), new Claim(ProviderClaims.UserId, UserId)}, 
                "mock"));
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
                   c.RouteName.Equals(RouteNames.GetRemoveProviderCourseLocation)
               )))
               .Returns(verifyUrl)
               .Callback<UrlRouteContext>(c =>
               {
                   verifyRouteValues = c;
               });
            _sut.Url = urlHelper.Object;
        }

        [Test, AutoData]
        public async Task Post_ValidModel_RedirectToRoute(ProviderCourseLocationViewModel model, GetProviderCourseLocationsQueryResult queryResult)
        {
            _mediatorMock
               .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == model.LarsCode), It.IsAny<CancellationToken>()))
               .ReturnsAsync(queryResult);

            var result =  await _sut.RemoveProviderCourseLocation(model);

            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteProviderCourseLocationCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            var redirectResult = result as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(model.BackLink);
        }
    }
}
