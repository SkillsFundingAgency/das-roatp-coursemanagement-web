using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard
{
    public class ConfirmStandardSubmitModelValidator : AbstractValidator<ConfirmNonRegulatedStandardSubmitModel>
    {
        public const string ConfirmationAnswerMissingMessage = "Answer the question by selecting either yes or no";
        public ConfirmStandardSubmitModelValidator()
        {
            RuleFor(c => c.IsCorrectStandard)
                .NotEmpty()
                .WithMessage(ConfirmationAnswerMissingMessage);
        }
    }
}
