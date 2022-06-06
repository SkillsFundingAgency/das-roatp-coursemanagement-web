using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmRegulatedStandardControllerTests
{
    [TestFixture]
    public class ConfirmRegulatedStandardControllerGetTests
    {
        private const string Ukprn = "10012002";
        private Mock<ILogger<ConfirmRegulatedStandardController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<IUrlHelper> _urlHelperMock;
        private ConfirmRegulatedStandardController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<ConfirmRegulatedStandardController>>();
            _mediatorMock = new Mock<IMediator>();
            _urlHelperMock = new Mock<IUrlHelper>();
           
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), }, "mock"));
            var httpContext = new DefaultHttpContext() { User = user };
            _sut = new ConfirmRegulatedStandardController(_mediatorMock.Object, _loggerMock.Object)
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
            GetStandardDetailsQueryResult queryResult,
            int larsCode)
        {
            string detailsUrl = $"{Ukprn}/standards/{larsCode}/confirm-regulated-standard";
           
            _urlHelperMock
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.ViewStandardDetails))))
               .Returns(detailsUrl);

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);


            _sut.HttpContext.Request.Headers.Add("Referer", detailsUrl);

            var result = await _sut.ConfirmRegulatedStandard(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ConfirmRegulatedStandardViewModel;
            model.Should().NotBeNull();
            model.BackLink.Should().Be(detailsUrl);
            model.CancelLink.Should().Be(detailsUrl);
        }

        [Test, AutoData]
        public async Task Get_InvalidRequest_ThrowsInvalidOperationException(int larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetStandardDetailsQueryResult)null);

            Func<Task> action = () => _sut.ConfirmRegulatedStandard(larsCode);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
