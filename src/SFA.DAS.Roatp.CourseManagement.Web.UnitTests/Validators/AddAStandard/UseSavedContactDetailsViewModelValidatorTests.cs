using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddAStandard;
public class UseSavedContactDetailsViewModelValidatorTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void UseSavedContactDetails_Valid_NoErrors(bool value)
    {
        var model = new UseSavedContactDetailsViewModel { Ukprn = 123, IsUsingSavedContactDetails = value };
        var sut = new UseSavedContactDetailsSubmitViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.IsUsingSavedContactDetails);
    }

    [Test]
    public void UseSavedContactDetails_Invalid_WithExpectedError()
    {
        var model = new UseSavedContactDetailsViewModel { Ukprn = 123 };
        var sut = new UseSavedContactDetailsSubmitViewModelValidator();

        var result = sut.TestValidate(model);

        result
            .ShouldHaveValidationErrorFor(a => a.IsUsingSavedContactDetails)
            .WithErrorMessage(UseSavedContactDetailsSubmitViewModelValidator.UseSavedContactDetailsAnswerMissingMessage);
    }
}
