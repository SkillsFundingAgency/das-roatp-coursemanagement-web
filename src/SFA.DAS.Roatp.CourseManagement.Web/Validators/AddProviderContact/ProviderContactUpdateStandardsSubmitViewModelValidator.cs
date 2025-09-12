using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddProviderContact;

public class ProviderContactUpdateStandardsSubmitViewModelValidator : AbstractValidator<ConfirmUpdateStandardsSubmitViewModel>
{
    public const string UpdateStandardsNotPickedMessage = "Select whether you want to use these contact details for your standards";
    public const string UpdateStandardsPhoneOnlyNotPickedMessage = "Select whether you want to use this phone number for your standards";
    public const string UpdateStandardsEmailOnlyNotPickedMessage = "Select whether you want to use this email for your standards";

    public ProviderContactUpdateStandardsSubmitViewModelValidator()
    {
        RuleFor(p => p.HasOptedToUpdateExistingStandards)
            .Cascade(CascadeMode.Continue)
            .NotEmpty()
            .WithMessage(UpdateStandardsNotPickedMessage).When(r =>
                !string.IsNullOrEmpty(r.EmailAddress) && !string.IsNullOrEmpty(r.PhoneNumber));

        RuleFor(p => p.HasOptedToUpdateExistingStandards)
            .Cascade(CascadeMode.Continue)
            .NotEmpty()
            .WithMessage(UpdateStandardsEmailOnlyNotPickedMessage)
            .When(r => string.IsNullOrEmpty(r.PhoneNumber) && !string.IsNullOrEmpty(r.EmailAddress));

        RuleFor(p => p.HasOptedToUpdateExistingStandards)
            .Cascade(CascadeMode.Continue)
            .NotEmpty()
            .WithMessage(UpdateStandardsPhoneOnlyNotPickedMessage)
            .When(r => !string.IsNullOrEmpty(r.PhoneNumber) && string.IsNullOrEmpty(r.EmailAddress));
    }
}