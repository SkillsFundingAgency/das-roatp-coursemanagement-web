using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditCourseContactDetailsControllerTests
{
    [TestFixture]
    public class EditCourseContactDetailsControllerPostTests
    {
        private const string Ukprn = "10012002";
        private static string UserId = Guid.NewGuid().ToString();
        private static string DetailsUrl = Guid.NewGuid().ToString();
        private Mock<ILogger<EditCourseContactDetailsController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<IUrlHelper> _urlHelperMock;
        private EditCourseContactDetailsController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<EditCourseContactDetailsController>>();
            _mediatorMock = new Mock<IMediator>();
            _urlHelperMock = new Mock<IUrlHelper>();
            _urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.ViewStandardDetails))))
               .Returns(DetailsUrl);

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), new Claim(ProviderClaims.UserId, UserId)}, 
                "mock"));
            var httpContext = new DefaultHttpContext() { User = user };
            _sut = new EditCourseContactDetailsController(_mediatorMock.Object, _loggerMock.Object)
            {
                Url = _urlHelperMock.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test, AutoData]
        public async Task Post_ValidModel_SendsUpdateCommand(EditCourseContactDetailsViewModel model)
        {
            var result = await _sut.Index(model);

            _mediatorMock.Verify(m => m.Send(It.Is<UpdateProviderCourseContactDetailsCommand>(c => c.Ukprn == int.Parse(Ukprn) && c.UserId == UserId), It.IsAny<CancellationToken>()));
            var routeResult = result as RedirectToRouteResult;
            routeResult.Should().NotBeNull();
            routeResult.RouteName.Should().Be(RouteNames.ViewStandardDetails);
            routeResult.RouteValues.Should().NotBeEmpty().And.HaveCount(2);
            routeResult.RouteValues.Should().ContainKey("ukprn").WhoseValue.Should().Be(int.Parse(Ukprn));
            routeResult.RouteValues.Should().ContainKey("larsCode").WhoseValue.Should().Be(model.LarsCode);
        }

        [Test, AutoData]
        public async Task Post_InValidModel_ReturnsView(EditCourseContactDetailsViewModel model)
        {
            _sut.ModelState.AddModelError("key", "error");

            var result = await _sut.Index(model);

            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateProviderCourseContactDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Never);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
            model.BackLink.Should().Be(DetailsUrl);
            model.CancelLink.Should().Be(DetailsUrl);
        }
    }
}
