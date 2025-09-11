using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

public class UseSavedContactDetailsSubmitViewModelValidator
    : AbstractValidator<UseSavedContactDetailsSubmitViewModel>
{
    public const string UseSavedContactDetailsAnswerMissingMessage = "Select yes if you want to use your saved contact details";
    public UseSavedContactDetailsSubmitViewModelValidator()
    {
        RuleFor(c => c.IsUsingSavedContactDetails)
            .NotEmpty()
            .WithMessage(UseSavedContactDetailsAnswerMissingMessage);
    }
}
