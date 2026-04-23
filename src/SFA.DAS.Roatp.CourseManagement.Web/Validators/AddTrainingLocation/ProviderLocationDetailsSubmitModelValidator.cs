using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Domain;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation;

public class ProviderLocationDetailsSubmitModelValidator : AbstractValidator<ProviderLocationDetailsSubmitModel>
{
    public const string LocationNameMissingMessage = "Enter a venue name";
    public const string LocationNameCharacterLimitMessage = "Venue name must be 50 characters or less";
    public const string HasExcludedCharactersInVenueNameMessage = "Invalid characters in venue name";

    public ProviderLocationDetailsSubmitModelValidator()
    {
        RuleFor(m => m.LocationName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(LocationNameMissingMessage)
            .MaximumLength(50)
            .WithMessage(LocationNameCharacterLimitMessage)
            .Matches(Constants.RegularExpressions.ExcludedCharactersRegex)
            .WithMessage(HasExcludedCharactersInVenueNameMessage);
    }
}
