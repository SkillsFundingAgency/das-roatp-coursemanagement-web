using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Application.Services;
using SFA.DAS.Roatp.CourseManagement.Domain;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddProviderContact;

public class AddProviderContactSubmitViewModelValidator : AbstractValidator<AddProviderContactSubmitViewModel>
{
    public const string NoEmailOrPhoneNumberErrorMessage = "You must enter an email address or phone number";
    public const string InvalidDomainErrorMessage = "Enter an email address with a valid domain";

    public AddProviderContactSubmitViewModelValidator()
    {
        RuleFor(p => p.EmailAddress)
            .Cascade(CascadeMode.Stop)
            .Matches(Constants.RegularExpressions.EmailRegex)
            .WithMessage(CommonValidationErrorMessage.EmailInvalidMessage)
            .Must(IsDomainValid)
            .WithMessage(InvalidDomainErrorMessage);

        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .When(a => string.IsNullOrWhiteSpace(a.PhoneNumber))
            .WithMessage(NoEmailOrPhoneNumberErrorMessage);

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .When(a => string.IsNullOrWhiteSpace(a.EmailAddress))
            .WithMessage(NoEmailOrPhoneNumberErrorMessage)
            .Matches(Constants.RegularExpressions.ExcludedCharactersRegex)
            .WithMessage(CommonValidationErrorMessage.TelephoneHasExcludedCharacter)
            .MinimumLength(10)
            .WithMessage(CommonValidationErrorMessage.TelephoneLengthMessage)
            .MaximumLength(50)
            .WithMessage(CommonValidationErrorMessage.TelephoneLengthMessage);
    }

    private static bool IsDomainValid(string email)
    {
        return string.IsNullOrEmpty(email) ||
               EmailCheckingService.IsValidDomain(email);
    }
}