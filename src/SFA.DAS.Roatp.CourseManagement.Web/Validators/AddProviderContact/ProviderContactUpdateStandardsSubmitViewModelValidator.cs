using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddProviderContact;

public class ProviderContactUpdateStandardsSubmitViewModelValidator : AbstractValidator<ProviderContactUpdateStandardsSubmitViewModel>
{
    public const string UpdateStandardsNotPickedMessage = "Select whether you want to use these contact details for your standards";

    public ProviderContactUpdateStandardsSubmitViewModelValidator()
    {
        RuleFor(p => p.UpdateExistingStandards)
            .NotEmpty()
            .WithMessage(UpdateStandardsNotPickedMessage);
    }
}