using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Domain;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class ConfirmRegulatedStandardViewModelValidator : AbstractValidator<ConfirmRegulatedStandardViewModel>
    {
        public const string IsApprovedByRegulatorErrorMessage = "Tell us if you have been approved by the regulator";
        public ConfirmRegulatedStandardViewModelValidator()
        {
            RuleFor(p => p.IsApprovedByRegulator)
                .NotEmpty()
                .WithMessage(IsApprovedByRegulatorErrorMessage);
        }
    }
}
