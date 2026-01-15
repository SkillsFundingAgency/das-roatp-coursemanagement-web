using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAnApprenticeshipUnit;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddAnApprenticeshipUnit;
public class ConfirmApprenticeshipUnitSubmitModelValidatorTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void IsCorrectShortCourse_Valid_NoErrors(bool value)
    {
        var model = new ConfirmApprenticeshipUnitSubmitModel { IsCorrectShortCourse = value };
        var sut = new ConfirmApprenticeshipUnitSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.IsCorrectShortCourse);
    }

    [Test]
    public void IsCorrectShortCourse_Invalid_NoErrors()
    {
        var model = new ConfirmApprenticeshipUnitSubmitModel();
        var sut = new ConfirmApprenticeshipUnitSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.IsCorrectShortCourse);
    }
}