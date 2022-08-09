using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddAStandard
{
    [TestFixture]
    public class ConfirmStandardSubmitModelValidatorTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void IsCorrectStandard_Valid_NoErrors(bool value)
        {
            var model = new ConfirmNonRegulatedStandardSubmitModel { IsCorrectStandard = value };
            var sut = new ConfirmStandardSubmitModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.IsCorrectStandard);
        }

        [Test]
        public void IsCorrectStandard_Invalid_NoErrors()
        {
            var model = new ConfirmNonRegulatedStandardSubmitModel();
            var sut = new ConfirmStandardSubmitModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.IsCorrectStandard);
        }
    }
}
