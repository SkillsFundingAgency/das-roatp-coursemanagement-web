using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ConfirmRegulatedStandardViewModelValidatorTests
{
    [TestFixture]
    public class ValidateIsApprovedByRegulatorTests
    {
        [Test]
        public void WhenNull_ProducesValidatonError()
        {
            var sut = new ConfirmRegulatedStandardViewModelValidator();

            var command = new ConfirmRegulatedStandardViewModel();

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.IsApprovedByRegulator).WithErrorMessage("Select Yes or No");
        }
    }
}
