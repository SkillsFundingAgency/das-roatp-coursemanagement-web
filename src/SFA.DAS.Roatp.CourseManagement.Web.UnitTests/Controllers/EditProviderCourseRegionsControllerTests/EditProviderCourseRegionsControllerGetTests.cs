using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries.GetAllStandardRegions;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditProviderCourseRegionsControllerTests
{
    [TestFixture]
    public class EditProviderCourseRegionsControllerGetTests
    {
        private const string Ukprn = "10012002";
        private static string DetailsUrl = Guid.NewGuid().ToString();
        private Mock<ILogger<EditProviderCourseRegionsController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<IUrlHelper> _urlHelperMock;
        private EditProviderCourseRegionsController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<EditProviderCourseRegionsController>>();
            _mediatorMock = new Mock<IMediator>();
            _urlHelperMock = new Mock<IUrlHelper>();
            _urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.ViewStandardDetails))))
               .Returns(DetailsUrl);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), }, "mock"));
            var httpContext = new DefaultHttpContext() { User = user };
            _sut = new EditProviderCourseRegionsController(_mediatorMock.Object, _loggerMock.Object)
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
            GetAllStandardRegionsQueryResult queryResult,
            int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllStandardRegionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetAllRegions(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as RegionsViewModel;
            model.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task Get_ValidRequestWithReferer_ReturnsValidBackAndCancelLinks(
           GetAllStandardRegionsQueryResult queryResult,
           int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetAllStandardRegionsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
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
        public async Task Get_ValidRequestNoRegions_RedirectToNotFoundPage(
           GetAllStandardRegionsQueryResult queryResult,
           int larsCode)
        {
            queryResult.Regions = new System.Collections.Generic.List<Domain.ApiModels.Region>();
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetAllStandardRegionsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var expectedUrl = $"Error/{HttpStatusCode.NotFound}";
            var result = await _sut.GetAllRegions(larsCode);

            var redirectResult = result as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(expectedUrl);
        }

        [Test, AutoData]
        public async Task Get_InvalidRequest_ThrowsInvalidOperationException(int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllStandardRegionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetAllStandardRegionsQueryResult)null);

            Func<Task> action = () => _sut.GetAllRegions(larsCode);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
