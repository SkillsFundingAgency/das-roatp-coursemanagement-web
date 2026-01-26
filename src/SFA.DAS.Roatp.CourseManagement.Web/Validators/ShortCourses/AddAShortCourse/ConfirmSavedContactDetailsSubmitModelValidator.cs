using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.AddAShortCourse;

public class ConfirmSavedContactDetailsSubmitModelValidator : AbstractValidator<ConfirmSavedContactDetailsSubmitModel>
{
    public const string ConfirmSavedContactDetailsAnswerMissingMessage = "You must select which contact details to use";
    public ConfirmSavedContactDetailsSubmitModelValidator()
    {
        RuleFor(c => c.IsUsingSavedContactDetails)
            .NotEmpty()
            .WithMessage(ConfirmSavedContactDetailsAnswerMissingMessage);
    }
}