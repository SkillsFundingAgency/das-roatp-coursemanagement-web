using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard
{
    public class ConfirmNewRegulatedStandardSubmitModelValidator : AbstractValidator<ConfirmNewRegulatedStandardSubmitModel>
    {
        public const string ConfirmationAnswerMissingMessage = "Tell us if you have been approved by the regulator";
        public ConfirmNewRegulatedStandardSubmitModelValidator()
        {
            RuleFor(c => c.IsCorrectStandard)
                .NotEmpty()
                .WithMessage(ConfirmationAnswerMissingMessage);
        }
    }
}