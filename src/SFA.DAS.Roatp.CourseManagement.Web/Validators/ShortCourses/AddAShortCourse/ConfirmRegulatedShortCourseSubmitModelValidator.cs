using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.AddAShortCourse;

public class ConfirmRegulatedShortCourseSubmitModelValidator : AbstractValidator<ConfirmRegulatedShortCourseSubmitModel>
{
    public const string ConfirmationAnswerMissingMessage = "Tell us if you have been approved by the regulator";
    public ConfirmRegulatedShortCourseSubmitModelValidator()
    {
        RuleFor(c => c.IsApprovedByRegulator)
            .NotEmpty()
            .WithMessage(ConfirmationAnswerMissingMessage);
    }
}
