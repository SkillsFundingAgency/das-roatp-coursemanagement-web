using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddAShortCourse;
public class ConfirmSavedContactDetailsSubmitModelValidatorTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void IsUsingSavedContactDetails_Valid_NoErrors(bool value)
    {
        var model = new ConfirmSavedContactDetailsSubmitModel { IsUsingSavedContactDetails = value };
        var sut = new ConfirmSavedContactDetailsSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.IsUsingSavedContactDetails);
    }

    [Test]
    public void IsUsingSavedContactDetails_Invalid_WithExpectedError()
    {
        var model = new ConfirmSavedContactDetailsSubmitModel();
        var sut = new ConfirmSavedContactDetailsSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.IsUsingSavedContactDetails);
    }
}
