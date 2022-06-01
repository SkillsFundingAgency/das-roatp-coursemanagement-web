using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Domain;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class ConfirmRegulatedStandardViewModelValidator : AbstractValidator<ConfirmRegulatedStandardViewModel>
    {
        public const string TelephoneLengthErrorMessage = "Telephone number must be between 10 and 50 characters";
        public ConfirmRegulatedStandardViewModelValidator()
        {
            RuleFor(p => p.IsApprovedByRegulator)
                .NotEmpty()
                .WithMessage("Select Yes or No");
        }
    }
}
