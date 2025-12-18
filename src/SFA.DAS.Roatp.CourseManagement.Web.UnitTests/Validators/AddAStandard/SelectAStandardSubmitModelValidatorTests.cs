using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddAStandard
{
    [TestFixture]
    public class SelectAStandardSubmitModelValidatorTests
    {
        [TestCase("0", true)]
        [TestCase("12", true)]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
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
    }
}