using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddShortCourses;

public class ConfirmShortCourseSubmitModelValidator : AbstractValidator<ConfirmShortCourseSubmitModel>
{
    public const string ConfirmationAnswerMissingMessage = "You must select if this is the right apprenticeship unit or not";
    public ConfirmShortCourseSubmitModelValidator()
    {
        RuleFor(c => c.IsCorrectShortCourse)
            .NotEmpty()
            .WithMessage(ConfirmationAnswerMissingMessage);
    }
}