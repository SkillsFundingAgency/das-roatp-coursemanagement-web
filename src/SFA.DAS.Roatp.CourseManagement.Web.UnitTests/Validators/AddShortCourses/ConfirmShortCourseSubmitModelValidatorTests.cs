using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddShortCourses;
public class ConfirmShortCourseSubmitModelValidatorTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void IsCorrectShortCourse_Valid_NoErrors(bool value)
    {
        var model = new ConfirmShortCourseSubmitModel { IsCorrectShortCourse = value };
        var sut = new ConfirmShortCourseSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.IsCorrectShortCourse);
    }

    [Test]
    public void IsCorrectShortCourse_Invalid_WithExpectedError()
    {
        var model = new ConfirmShortCourseSubmitModel();
        var sut = new ConfirmShortCourseSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.IsCorrectShortCourse);
    }
}