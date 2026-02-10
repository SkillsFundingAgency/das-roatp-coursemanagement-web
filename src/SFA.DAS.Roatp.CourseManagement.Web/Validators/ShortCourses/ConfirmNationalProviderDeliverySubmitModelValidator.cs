using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses;

public class ConfirmNationalProviderDeliverySubmitModelValidator : AbstractValidator<ConfirmNationalProviderDeliverySubmitModel>
{
    public const string ErrorMessage = "Select whether you can deliver this training at employers’ addresses anywhere in England";
    public ConfirmNationalProviderDeliverySubmitModelValidator()
    {
        RuleFor(m => m.HasNationalDeliveryOption)
            .NotNull()
            .WithMessage(ErrorMessage);
    }
}
