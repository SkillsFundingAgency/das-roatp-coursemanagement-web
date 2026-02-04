using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ShortCourses.AddAShortCourse;
public class ConfirmShortCourseSubmitModelValidatorTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void IsCorrectShortCourse_Valid_NoErrors(bool value)
    {
        var courseType = CourseType.ShortCourse;
        var model = new ConfirmShortCourseSubmitModel { IsCorrectShortCourse = value, CourseType = courseType };
        var sut = new ConfirmShortCourseSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.IsCorrectShortCourse);
    }

    [Test]
    public void IsCorrectShortCourse_Invalid_WithExpectedError()
    {
        var courseType = CourseType.ShortCourse;
        var model = new ConfirmShortCourseSubmitModel() { CourseType = courseType };
        var sut = new ConfirmShortCourseSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.IsCorrectShortCourse);
    }
}