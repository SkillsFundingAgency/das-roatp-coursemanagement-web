using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses;

public class ConfirmNationalDeliverySubmitModelValidator : AbstractValidator<ConfirmNationalDeliverySubmitModel>
{
    public const string ErrorMessage = "Select whether you can deliver this training at employers’ addresses anywhere in England";
    public ConfirmNationalDeliverySubmitModelValidator()
    {
        RuleFor(m => m.HasNationalDeliveryOption)
            .NotNull()
            .WithMessage(ErrorMessage);
    }
}
