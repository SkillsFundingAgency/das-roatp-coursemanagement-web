using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ShortCourses.AddAShortCourse;

public class ConfirmRegulatedShortCourseSubmitModelValidatorTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void IsApprovedByRegulator_Valid_NoErrors(bool value)
    {
        var model = new ConfirmRegulatedShortCourseSubmitModel { IsApprovedByRegulator = value };
        var sut = new ConfirmRegulatedShortCourseSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.IsApprovedByRegulator);
    }

    [Test]
    public void IsApprovedByRegulator_Invalid_WithExpectedError()
    {
        var model = new ConfirmRegulatedShortCourseSubmitModel();
        var sut = new ConfirmRegulatedShortCourseSubmitModelValidator();

        var result = sut.TestValidate(model);

        result
            .ShouldHaveValidationErrorFor(a => a.IsApprovedByRegulator)
            .WithErrorMessage(ConfirmRegulatedShortCourseSubmitModelValidator.ConfirmationAnswerMissingMessage);
    }
}
