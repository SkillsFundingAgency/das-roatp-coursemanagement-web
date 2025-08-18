using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddProviderContact;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddProviderContact;

[TestFixture]
public class AddProviderContactSubmitViewModelValidatorTests
{
    [TestCase("test@test.com", true)]
    [TestCase("avcx", false)]
    [TestCase(".com", false)]
    public async Task EmailRegexChecked(string email, bool isValid)
    {
        var model = new AddProviderContactSubmitViewModel { EmailAddress = email };

        var sut = new AddProviderContactSubmitViewModelValidator();
        var result = await sut.TestValidateAsync(model);

        if (isValid)
            result.ShouldNotHaveAnyValidationErrors();
        else
            result.ShouldHaveValidationErrorFor(m => m.EmailAddress).WithErrorMessage(CommonValidationErrorMessage.EmailInvalidMessage);
    }

    [TestCase("test@account.com", "")]
    [TestCase("test//@account.com", "")]
    [TestCase("test", CommonValidationErrorMessage.EmailInvalidMessage)]
    [TestCase("test.com", CommonValidationErrorMessage.EmailInvalidMessage)]
    [TestCase("test test@account.com", CommonValidationErrorMessage.EmailInvalidMessage)]
    [TestCase("aaaa@NonExistentDomain50c2413d-e8e4-4330-9859-222567ad0f64.co.uk", AddProviderContactSubmitViewModelValidator.InvalidDomainErrorMessage)]
    public async Task ValidEmailInModel_IsValid(string email, string validationMessage)
    {
        var model = new AddProviderContactSubmitViewModel()
        {
            EmailAddress = email,
            PhoneNumber = "123"
        };

        var sut = new AddProviderContactSubmitViewModelValidator();
        var result = await sut.TestValidateAsync(model);

        if (validationMessage == string.Empty)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.EmailAddress)
                .WithErrorMessage(validationMessage);
        }
    }

    [Test]
    public void EmailAndPostcodeIsEmpty()
    {
        var model = new AddProviderContactSubmitViewModel();
        var sut = new AddProviderContactSubmitViewModelValidator();

        var result = sut.TestValidate(model);

        result.Errors.Count.Should().Be(2);
        result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
            .WithErrorMessage(AddProviderContactSubmitViewModelValidator.NoEmailOrPhoneNumberErrorMessage);
        result.ShouldHaveValidationErrorFor(m => m.PhoneNumber)
            .WithErrorMessage(AddProviderContactSubmitViewModelValidator.NoEmailOrPhoneNumberErrorMessage);
    }
}