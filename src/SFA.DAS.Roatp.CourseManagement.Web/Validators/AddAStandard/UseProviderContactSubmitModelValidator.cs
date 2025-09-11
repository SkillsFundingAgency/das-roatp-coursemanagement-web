using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

public class UseSavedContactDetailsViewModelValidator
    : AbstractValidator<UseSavedContactDetailsViewModel>
{
    public const string ConfirmationAnswerMissingMessage = "Select yes if you want to use your saved contact details";
    public UseSavedContactDetailsViewModelValidator()
    {
        RuleFor(c => c.IsUsingSavedContactDetails)
            .NotEmpty()
            .WithMessage(ConfirmationAnswerMissingMessage);
    }
}
