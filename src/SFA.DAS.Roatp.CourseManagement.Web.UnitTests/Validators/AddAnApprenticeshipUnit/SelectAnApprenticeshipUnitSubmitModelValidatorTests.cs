using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAnApprenticeshipUnit;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddAnApprenticeshipUnit;
public class SelectAnApprenticeshipUnitSubmitModelValidatorTests
{
    [TestCase("0", true)]
    [TestCase("12", true)]
    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase(" ", false)]
    public void SelectedLarsCode_Validation(string larsCode, bool isValid)
    {
        var model = new SelectAnApprenticeshipUnitSubmitModel() { SelectedLarsCode = larsCode };
        var sut = new SelectAnApprenticeshipUnitSubmitModelValidator();

        var result = sut.TestValidate(model);

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(m => m.SelectedLarsCode);
        else
            result.ShouldHaveValidationErrorFor(m => m.SelectedLarsCode);
    }
}