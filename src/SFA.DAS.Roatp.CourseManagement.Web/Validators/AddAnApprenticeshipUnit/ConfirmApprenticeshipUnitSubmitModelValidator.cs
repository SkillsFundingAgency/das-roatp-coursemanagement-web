using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAnApprenticeshipUnit;

public class ConfirmApprenticeshipUnitSubmitModelValidator : AbstractValidator<ConfirmApprenticeshipUnitSubmitModel>
{
    public const string ConfirmationAnswerMissingMessage = "You must select if this is the right apprenticeship unit or not";
    public ConfirmApprenticeshipUnitSubmitModelValidator()
    {
        RuleFor(c => c.IsCorrectShortCourse)
            .NotEmpty()
            .WithMessage(ConfirmationAnswerMissingMessage);
    }
}