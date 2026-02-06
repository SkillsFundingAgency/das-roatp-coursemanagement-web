using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.ManageShortCourses;

public class AddTrainingVenueSubmitModelValidator : AbstractValidator<AddTrainingVenueSubmitModel>
{

    public const string MissingAddressErrorMessage = "Enter the first 3 letters of an address or postcode and select a location.";

    public AddTrainingVenueSubmitModelValidator()
    {
        When(m => string.IsNullOrWhiteSpace(m.Postcode), () =>
        {
            RuleFor(e => e.SearchTerm)
                .NotEmpty()
                .WithMessage(MissingAddressErrorMessage);
        });
    }
}