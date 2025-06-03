using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation;

public class AddressSearchSubmitModelValidator : AbstractValidator<AddressSearchSubmitModel>
{

    public const string MissingAddressErrorMessage = "Enter the first 3 letters of an address or postcode and select a location.";

    public AddressSearchSubmitModelValidator()
    {
        When(m => string.IsNullOrWhiteSpace(m.Postcode), () =>
        {
            RuleFor(e => e.SearchTerm)
                .NotEmpty()
                .WithMessage(MissingAddressErrorMessage);
        });
    }
}