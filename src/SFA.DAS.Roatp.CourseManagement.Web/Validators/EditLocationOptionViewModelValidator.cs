using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class EditLocationOptionViewModelValidator : AbstractValidator<EditLocationOptionViewModel>
    {
        public EditLocationOptionViewModelValidator()
        {
            RuleFor(m => m.LocationOption)
                .NotNull()
                .WithMessage("Select where you will deliver this standard");
        }
    }
}
