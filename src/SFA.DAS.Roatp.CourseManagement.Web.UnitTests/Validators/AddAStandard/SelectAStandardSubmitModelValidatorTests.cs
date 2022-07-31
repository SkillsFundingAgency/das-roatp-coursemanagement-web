using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddAStandard
{
    [TestFixture]
    public class SelectAStandardSubmitModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(12, true)]
        public void SelectedLarsCode_Validation(int larsCode, bool isValid)
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
