using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.ManageShortCourses;

public class ConfirmAddTrainingVenueSubmitModelValidator : AbstractValidator<ConfirmAddTrainingVenueSubmitModel>
{
    public const string VenueNameMissingMessage = "Enter a name";

    public ConfirmAddTrainingVenueSubmitModelValidator()
    {
        RuleFor(m => m.LocationName)
            .NotEmpty()
            .WithMessage(VenueNameMissingMessage);
    }
}
