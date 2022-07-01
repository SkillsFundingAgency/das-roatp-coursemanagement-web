using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries.GetAllRegions;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditRegionsControllerTests
{
    [TestFixture]
    public class EditRegionsControllerPostTests
    {
        private const string Ukprn = "10012002";
        private static string UserId = Guid.NewGuid().ToString();
        private static string DetailsUrl = Guid.NewGuid().ToString();
        private Mock<ILogger<EditRegionsController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<IUrlHelper> _urlHelperMock;
        private EditRegionsController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<EditRegionsController>>();
            _mediatorMock = new Mock<IMediator>();
            _urlHelperMock = new Mock<IUrlHelper>();
            _urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.ViewStandardDetails))))
               .Returns(DetailsUrl);

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), new Claim(ProviderClaims.UserId, UserId)}, 
                "mock"));
            var httpContext = new DefaultHttpContext() { User = user };

            _sut = new EditRegionsController(_mediatorMock.Object, _loggerMock.Object)
            {
                Url = _urlHelperMock.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test, AutoData]
        public async Task Post_ValidModel_SendsUpdateCommand(RegionsViewModel model)
        {
            string[] selectedSubRegions = new string[] { "1", "2" };
            model.SelectedSubRegions = selectedSubRegions;

            var result =  await _sut.UpdateSubRegions(model);
            var routeResult = result as RedirectToRouteResult;
            routeResult.Should().NotBeNull();
            routeResult.RouteName.Should().Be(RouteNames.ViewStandardDetails);
            routeResult.RouteValues.Should().NotBeEmpty().And.HaveCount(2);
            routeResult.RouteValues.Should().ContainKey("ukprn").WhoseValue.Should().Be(int.Parse(Ukprn));
            routeResult.RouteValues.Should().ContainKey("larsCode").WhoseValue.Should().Be(model.LarsCode);
        }

        [Test, AutoData]
        public async Task Post_InValidData_RedirectToSameView(RegionsViewModel model, GetAllRegionsQueryResult queryResult)
        {
            var backLink = model.BackUrl;
            var cancelLink = model.CancelLink;
            _sut.ModelState.AddModelError("key", "error");
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllRegionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);
            model.SelectedSubRegions = null;
            var result =  await _sut.UpdateSubRegions(model);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("EditRegions.cshtml");
            model.BackUrl.Should().Be(backLink);
            model.CancelLink.Should().Be(cancelLink);
        }
    }
}
