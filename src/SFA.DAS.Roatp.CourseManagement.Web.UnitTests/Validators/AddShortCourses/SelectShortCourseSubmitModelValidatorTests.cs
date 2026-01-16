using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddShortCourses;
public class SelectShortCourseSubmitModelValidatorTests
{
    [TestCase("0", true)]
    [TestCase("12", true)]
    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase(" ", false)]
    public void SelectedLarsCode_Validation(string larsCode, bool isValid)
    {
        var model = new SelectShortCourseSubmitModel() { SelectedLarsCode = larsCode };
        var sut = new SelectShortCourseSubmitModelValidator();

        var result = sut.TestValidate(model);

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(m => m.SelectedLarsCode);
        else
            result.ShouldHaveValidationErrorFor(m => m.SelectedLarsCode);
    }
}