using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddAStandard
{
    [TestFixture]
    public class SelectAStandardSubmitModelValidatorTests
    {
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("0", false)]
        [TestCase("-1", false)]
        [TestCase("abc", false)]
        [TestCase("12.5", false)]
        [TestCase("1", true)]
        [TestCase("12", true)]
        [TestCase("123456", true)]
        public void SelectedLarsCode_Validation(string larsCode, bool isValid)
        {
            var model = new SelectAStandardSubmitModel() { SelectedLarsCode = larsCode };
            var sut = new SelectAStandardSubmitModelValidator();

            var result = sut.TestValidate(model);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(m => m.SelectedLarsCode);
            else
                result.ShouldHaveValidationErrorFor(m => m.SelectedLarsCode);
        }

        [Test]
        public void SelectedLarsCode_Empty_ReturnsCorrectErrorMessage()
        {
            var model = new SelectAStandardSubmitModel() { SelectedLarsCode = string.Empty };
            var sut = new SelectAStandardSubmitModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedLarsCode)
                .WithErrorMessage(SelectAStandardSubmitModelValidator.StandardIsRequiredMesssage);
        }

        [Test]
        public void SelectedLarsCode_Invalid_ReturnsCorrectErrorMessage()
        {
            var model = new SelectAStandardSubmitModel() { SelectedLarsCode = "0" };
            var sut = new SelectAStandardSubmitModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedLarsCode)
                .WithErrorMessage(SelectAStandardSubmitModelValidator.StandardIsRequiredMesssage);
        }
    }
}