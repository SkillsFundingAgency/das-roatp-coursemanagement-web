using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Domain;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class ConfirmRegulatedStandardViewModelValidator : AbstractValidator<ConfirmRegulatedStandardViewModel>
    {
        public const string IsApprovedByRegulatorErrorMessage = "Select Yes or No";
        public ConfirmRegulatedStandardViewModelValidator()
        {
            RuleFor(p => p.IsApprovedByRegulator)
                .NotEmpty()
                .WithMessage(IsApprovedByRegulatorErrorMessage);
        }
    }
}
