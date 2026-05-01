using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmRegulatedStandardControllerTests
{
    [TestFixture]
    public class ConfirmRegulatedStandardControllerPostTests
    {
        private const string Ukprn = "10012002";
        private static string UserId = Guid.NewGuid().ToString();
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

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), new Claim(ProviderClaims.UserId, UserId) },
                "mock"));
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
        public async Task Post_ValidModel_SendsUpdateCommand(ConfirmRegulatedStandardViewModel model)
        {
            _validatorMock.Setup(x => x.Validate(It.IsAny<ConfirmRegulatedStandardViewModel>())).Returns(new ValidationResult());

            model.IsApprovedByRegulator = true;
            var result = await _sut.UpdateApprovedByRegulator(model);
            var redirectResult = result as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(model.RefererLink);
        }

        [Test, AutoData]
        public async Task Post_ValidModelWithIsApprovedByRegulatorFalse_RedirectToShutterPage(ConfirmRegulatedStandardViewModel model)
        {
            _validatorMock.Setup(x => x.Validate(It.IsAny<ConfirmRegulatedStandardViewModel>())).Returns(new ValidationResult());

            model.IsApprovedByRegulator = false;
            var result = await _sut.UpdateApprovedByRegulator(model);
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
            viewResult.ViewName.Should().Contain("RegulatedStandardSeekApproval.cshtml");
        }

        [Test, AutoData]
        public async Task Post_InValidModel_ReturnsView(ConfirmRegulatedStandardViewModel model)
        {
            var validationResult = new ValidationResult();

            validationResult.Errors.Add(new ValidationFailure("Field", "Error"));

            _validatorMock.Setup(x => x.Validate(It.IsAny<ConfirmRegulatedStandardViewModel>())).Returns(validationResult);

            var result = await _sut.UpdateApprovedByRegulator(model);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
        }
        [Test, AutoData]
        public async Task Post_ValidModelWithIsRegulatedStandardFalse_RedirectToErrorPage(ConfirmRegulatedStandardViewModel model)
        {
            _validatorMock.Setup(x => x.Validate(It.IsAny<ConfirmRegulatedStandardViewModel>())).Returns(new ValidationResult());

            model.RegulatorName = string.Empty;
            var expectedUrl = "Error/NotFound";
            var result = await _sut.UpdateApprovedByRegulator(model);
            var redirectResult = result as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(expectedUrl);
        }
    }
}
