using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ConfirmRegulatedStandardViewModelValidatorTests
{
    [TestFixture]
    public class ValidateIsApprovedByRegulatorTests
    {
        private const string IsApprovedByRegulatorErrorMessage = "Select Yes or No";
        [Test]
        public void WhenNull_ProducesValidatonError()
        {
            var sut = new ConfirmRegulatedStandardViewModelValidator();

            var command = new ConfirmRegulatedStandardViewModel();

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.IsApprovedByRegulator).WithErrorMessage(IsApprovedByRegulatorErrorMessage);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void WhenValidModel_ProducesNoError(bool isApprovedByRegulator)
        {
            var sut = new ConfirmRegulatedStandardViewModelValidator();

            var command = new ConfirmRegulatedStandardViewModel()
            {
                IsApprovedByRegulator = isApprovedByRegulator
            };

            var result = sut.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
