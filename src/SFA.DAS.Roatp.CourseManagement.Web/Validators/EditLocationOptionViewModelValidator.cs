using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class EditLocationOptionViewModelValidator : AbstractValidator<EditLocationOptionViewModel>
    {
        public const string NoneSelectedErrorMessage = "Select where you will deliver this standard";

        public EditLocationOptionViewModelValidator()
        {
            RuleFor(m => m.LocationOption)
                .NotEqual(LocationOption.None)
                .WithMessage(NoneSelectedErrorMessage);
        }
    }
}
