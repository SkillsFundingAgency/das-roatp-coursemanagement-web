using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Security.Claims;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmRegulatedStandardControllerTests
{
    [TestFixture]
    public class ConfirmRegulatedStandardControllerPostTests
    {
        private const string Ukprn = "10012002";
        private static string UserId = Guid.NewGuid().ToString();
        private Mock<ILogger<ConfirmRegulatedStandardController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private ConfirmRegulatedStandardController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<ConfirmRegulatedStandardController>>();
            _mediatorMock = new Mock<IMediator>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), new Claim(ProviderClaims.UserId, UserId)}, 
                "mock"));
            var httpContext = new DefaultHttpContext() { User = user };

            _sut = new ConfirmRegulatedStandardController(_mediatorMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test, AutoData]
        public void Post_InValidModel_ReturnsView(ConfirmRegulatedStandardViewModel model)
        {
            var backLink = model.BackLink;
            var cancelLink = model.CancelLink;
            _sut.ModelState.AddModelError("key", "error");

            var result =  _sut.SubmitConfirmRegulatedStandard(model);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
            model.BackLink.Should().Be(backLink);
            model.CancelLink.Should().Be(cancelLink);
        }
    }
}
