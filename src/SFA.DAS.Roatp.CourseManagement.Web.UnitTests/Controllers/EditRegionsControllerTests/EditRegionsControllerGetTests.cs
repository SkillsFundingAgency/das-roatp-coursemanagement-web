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
    public class EditRegionsControllerGetTests
    {
        private const string Ukprn = "10012002";
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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), }, "mock"));
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
        public async Task Get_ValidRequest_ReturnsView(
            GetAllRegionsQueryResult queryResult,
            int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllRegionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetAllRegions(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as RegionsViewModel;
            model.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task Get_ValidRequestWithReferer_ReturnsValidBackAndCancelLinks(
           GetAllRegionsQueryResult queryResult,
           int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetAllRegionsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);


            var result = await _sut.GetAllRegions(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as RegionsViewModel;
            model.Should().NotBeNull();
            model.BackUrl.Should().Be(DetailsUrl);
            model.CancelLink.Should().Be(DetailsUrl);
        }

        [Test, AutoData]
        public async Task Get_InvalidRequest_ThrowsInvalidOperationException(int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllRegionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetAllRegionsQueryResult)null);

            Func<Task> action = () => _sut.GetAllRegions(larsCode);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
