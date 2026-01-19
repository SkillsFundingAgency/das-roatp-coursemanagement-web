using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddShortCourses;

public class ConfirmShortCourseSubmitModelValidator : AbstractValidator<ConfirmShortCourseSubmitModel>
{
    public ConfirmShortCourseSubmitModelValidator()
    {
        RuleFor(c => c.IsCorrectShortCourse)
            .NotEmpty()
            .WithMessage(c => $"You must select if this is the right {c.CourseTypeDescription} or not");
    }
}