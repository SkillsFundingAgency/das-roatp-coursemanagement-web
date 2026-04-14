using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators;
public class ConfirmAddTrainingVenueSubmitModelValidatorTests
{
    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void VenueNameIsRequired_Invalid_WithExpectedError(string locationName)
    {
        var model = new ConfirmAddTrainingVenueSubmitModel { LocationName = locationName };

        var sut = new ConfirmAddTrainingVenueSubmitModelValidator();

        var result = sut.TestValidateAsync(model).Result;

        result.ShouldHaveValidationErrorFor(m => m.LocationName).WithErrorMessage(ConfirmAddTrainingVenueSubmitModelValidator.VenueNameMissingMessage);
    }

    [Test]
    public void VenueNameIsPresent_Valid_NoErrors()
    {
        var model = new ConfirmAddTrainingVenueSubmitModel { LocationName = "test" };

        var sut = new ConfirmAddTrainingVenueSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
