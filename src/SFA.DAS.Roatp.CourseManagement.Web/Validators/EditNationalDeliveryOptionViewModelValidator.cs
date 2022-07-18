using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class EditNationalDeliveryOptionViewModelValidator : AbstractValidator<EditNationalDeliveryOptionViewModel>
    {
        public const string ErrorMessage = "Tell us if you can provide training anywhere in England.";
        public EditNationalDeliveryOptionViewModelValidator()
        {
            RuleFor(m => m.HasNationalDeliveryOption)
                .NotNull()
                .WithMessage(ErrorMessage);
        }
    }
}
