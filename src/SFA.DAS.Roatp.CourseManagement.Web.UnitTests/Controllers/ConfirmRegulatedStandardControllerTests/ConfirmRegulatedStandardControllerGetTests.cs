using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmRegulatedStandardControllerTests
{
    [TestFixture]
    public class ConfirmRegulatedStandardControllerGetTests
    {
        private const string Ukprn = "10012002";
        private Mock<ILogger<ConfirmRegulatedStandardController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<IValidator<ConfirmRegulatedStandardViewModel>> _validatorMock;
        private ConfirmRegulatedStandardController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<ConfirmRegulatedStandardController>>();
            _mediatorMock = new Mock<IMediator>();
            _validatorMock = new Mock<IValidator<ConfirmRegulatedStandardViewModel>>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), }, "mock"));
            var httpContext = new DefaultHttpContext() { User = user };
            _sut = new ConfirmRegulatedStandardController(_mediatorMock.Object, _loggerMock.Object, _validatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test, AutoData]
        public async Task Get_ValidRequest_ReturnsView(
            GetProviderCourseDetailsQueryResult queryResult,
            string larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.ConfirmRegulatedStandard(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ConfirmRegulatedStandardViewModel;
            model.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task Get_ValidRequestWithReferer_ReturnsValidModel(
           GetProviderCourseDetailsQueryResult queryResult,
           string larsCode)
        {
            string detailsUrl = $"{Ukprn}/standards/{larsCode}/confirm-regulated-standard";

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            _sut.HttpContext.Request.Headers.Append("Referer", detailsUrl);

            var result = await _sut.ConfirmRegulatedStandard(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ConfirmRegulatedStandardViewModel;
            model.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task Get_WhenRefererNull_ReturnsViewWithDefaultLinks(
          GetProviderCourseDetailsQueryResult queryResult,
          string larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);


            var result = await _sut.ConfirmRegulatedStandard(larsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ConfirmRegulatedStandardViewModel;
            model.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task Get_InvalidRequest_ThrowsInvalidOperationException(string larsCode)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderCourseDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetProviderCourseDetailsQueryResult)null);

            Func<Task> action = () => _sut.ConfirmRegulatedStandard(larsCode);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
