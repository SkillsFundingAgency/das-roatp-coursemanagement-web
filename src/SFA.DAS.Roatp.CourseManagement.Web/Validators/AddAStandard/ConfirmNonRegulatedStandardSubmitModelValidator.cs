using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard
{
    public class ConfirmNonRegulatedStandardSubmitModelValidator : AbstractValidator<ConfirmNonRegulatedStandardSubmitModel>
    {
        public const string ConfirmationAnswerMissingMessage = "Tell us if this is the correct standard";
        public ConfirmNonRegulatedStandardSubmitModelValidator()
        {
            RuleFor(c => c.IsCorrectStandard)
                .NotEmpty()
                .WithMessage(ConfirmationAnswerMissingMessage);
        }
    }
}
