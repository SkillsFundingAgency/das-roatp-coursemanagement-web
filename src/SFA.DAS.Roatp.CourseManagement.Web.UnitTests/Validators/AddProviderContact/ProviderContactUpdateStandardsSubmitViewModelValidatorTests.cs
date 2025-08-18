using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddProviderContact;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddProviderContact;

[TestFixture]
public class ProviderContactUpdateStandardsSubmitViewModelValidatorTests
{
    [Test]
    public void NoSelectionIsMade_ShowsExpectedErrorMessage()
    {
        var model = new ProviderContactUpdateStandardsSubmitViewModel();
        var sut = new ProviderContactUpdateStandardsSubmitViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.UpdateExistingStandards)
            .WithErrorMessage(ProviderContactUpdateStandardsSubmitViewModelValidator.UpdateStandardsNotPickedMessage);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void NoSelectionIsMade(bool selection)
    {
        var model = new ProviderContactUpdateStandardsSubmitViewModel { UpdateExistingStandards = selection };
        var sut = new ProviderContactUpdateStandardsSubmitViewModelValidator();
        var result = sut.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
