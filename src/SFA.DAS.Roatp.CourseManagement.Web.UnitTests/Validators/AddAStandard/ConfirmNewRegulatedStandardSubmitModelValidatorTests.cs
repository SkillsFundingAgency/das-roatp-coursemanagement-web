using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddAStandard
{
    public class ConfirmNewRegulatedStandardSubmitModelValidatorTests

    {
        [TestCase(true)]
        [TestCase(false)]
        public void IsCorrectStandard_Valid_NoErrors(bool value)
        {
            var model = new ConfirmNewRegulatedStandardSubmitModel { IsCorrectStandard = value };
            var sut = new ConfirmNewRegulatedStandardSubmitModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.IsCorrectStandard);
        }

        [Test]
        public void IsCorrectStandard_Invalid_WithExpectedError()
        {
            var model = new ConfirmNewRegulatedStandardSubmitModel();
            var sut = new ConfirmNewRegulatedStandardSubmitModelValidator();

            var result = sut.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(a => a.IsCorrectStandard)
                .WithErrorMessage(ConfirmNewRegulatedStandardSubmitModelValidator.ConfirmationAnswerMissingMessage);
        }
    }
}
