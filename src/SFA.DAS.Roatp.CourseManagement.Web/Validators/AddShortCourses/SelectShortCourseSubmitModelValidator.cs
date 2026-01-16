using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddShortCourses;

public class SelectShortCourseSubmitModelValidator : AbstractValidator<SelectShortCourseSubmitModel>
{
    public const string ShortCourseIsRequiredMesssage = "Select an apprenticeship unit";
    public SelectShortCourseSubmitModelValidator()
    {
        RuleFor(m => m.SelectedLarsCode)
            .NotEmpty()
            .WithMessage(ShortCourseIsRequiredMesssage);
    }
}
