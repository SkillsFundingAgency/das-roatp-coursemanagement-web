using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddProviderContact;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddProviderContact;

[TestFixture]
public class ProviderContactUpdateStandardsSubmitViewModelValidatorTests
{
    [Test]
    public void NoSelectionIsMade_EmailAndPhone_ShowsExpectedErrorMessage()
    {
        var model = new ConfirmUpdateStandardsSubmitViewModel { EmailAddress = "test@test.com", PhoneNumber = "123" };
        var sut = new ProviderContactUpdateStandardsSubmitViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.HasOptedToUpdateExistingStandards)
            .WithErrorMessage(ProviderContactUpdateStandardsSubmitViewModelValidator.UpdateStandardsNotPickedMessage);
    }

    [Test]
    public void NoSelectionIsMade_EmailOnly_ShowsExpectedErrorMessage()
    {
        var model = new ConfirmUpdateStandardsSubmitViewModel { EmailAddress = "test@test.com" };
        var sut = new ProviderContactUpdateStandardsSubmitViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.HasOptedToUpdateExistingStandards)
            .WithErrorMessage(ProviderContactUpdateStandardsSubmitViewModelValidator.UpdateStandardsEmailOnlyNotPickedMessage);
    }

    [Test]
    public void NoSelectionIsMade_PhoneOnly_ShowsExpectedErrorMessage()
    {
        var model = new ConfirmUpdateStandardsSubmitViewModel { PhoneNumber = "123" };
        var sut = new ProviderContactUpdateStandardsSubmitViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.HasOptedToUpdateExistingStandards)
            .WithErrorMessage(ProviderContactUpdateStandardsSubmitViewModelValidator.UpdateStandardsPhoneOnlyNotPickedMessage);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void SelectionIsMade_NoErrors(bool selection)
    {
        var model = new ConfirmUpdateStandardsSubmitViewModel { HasOptedToUpdateExistingStandards = selection, EmailAddress = "test@test.com", PhoneNumber = "123" };
        var sut = new ProviderContactUpdateStandardsSubmitViewModelValidator();
        var result = sut.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
