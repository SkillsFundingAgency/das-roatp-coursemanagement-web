using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators;

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
