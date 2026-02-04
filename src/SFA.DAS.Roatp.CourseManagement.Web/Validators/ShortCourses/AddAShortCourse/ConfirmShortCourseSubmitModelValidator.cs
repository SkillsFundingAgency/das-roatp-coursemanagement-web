using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.AddAShortCourse;

public class ConfirmShortCourseSubmitModelValidator : AbstractValidator<ConfirmShortCourseSubmitModel>
{
    public ConfirmShortCourseSubmitModelValidator()
    {
        RuleFor(c => c.IsCorrectShortCourse)
            .NotEmpty()
            .WithMessage(c => $"You must select if this is the right {c.CourseTypeLower} or not");
    }
}