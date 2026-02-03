using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.ManageShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ShortCourses.ManageShortCourses;
public class AddTrainingVenueSubmitModelValidatorTests
{
    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void PostcodeIsRequired_Invalid_WithExpectedError(string postcode)
    {
        var model = new AddTrainingVenueSubmitModel { Postcode = postcode };

        var sut = new AddTrainingVenueSubmitModelValidator();
        var result = sut.TestValidateAsync(model).Result;
        result.ShouldHaveValidationErrorFor(m => m.SearchTerm).WithErrorMessage(AddTrainingVenueSubmitModelValidator.MissingAddressErrorMessage);
    }

    [Test]
    public void PostcodeIsPresent_Valid_NoErrors()
    {
        var model = new AddTrainingVenueSubmitModel { Postcode = "CV1" };
        var sut = new AddTrainingVenueSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

}
