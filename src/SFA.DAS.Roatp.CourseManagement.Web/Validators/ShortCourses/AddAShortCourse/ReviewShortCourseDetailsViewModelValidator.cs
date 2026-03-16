using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.AddAShortCourse;

public class ReviewShortCourseDetailsViewModelValidator : AbstractValidator<ReviewShortCourseDetailsViewModel>
{
    public const string IncompleteErrorMessage = "You need to complete the rest of the questions before you can add this training provider";
    public ReviewShortCourseDetailsViewModelValidator()
    {
        RuleFor(s => s)
            .Must(s =>
            !string.IsNullOrWhiteSpace(s.ContactInformation.ContactUsEmail) &&
            !string.IsNullOrWhiteSpace(s.ContactInformation.ContactUsPhoneNumber) &&
            !string.IsNullOrWhiteSpace(s.ContactInformation.StandardInfoUrl))
            .WithMessage(s => $"Enter all contact details for this {s.ApprenticeshipTypeLower}");

        RuleFor(s => s.LocationInformation.DeliveryLocations)
            .NotEmpty()
            .WithMessage(s => $"Select training options for this {s.ApprenticeshipTypeLower}");

        RuleFor(s => s.LocationInformation.TrainingVenues)
            .NotEmpty()
            .WithMessage(IncompleteErrorMessage)
            .When(s =>
            s.LocationInformation.LocationOptions.Contains(ShortCourseLocationOption.ProviderLocation));

        RuleFor(s => s.LocationInformation.HasNationalDeliveryOption)
            .NotNull()
            .WithMessage(IncompleteErrorMessage)
            .When(s =>
            s.LocationInformation.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation));

        RuleFor(s => s.LocationInformation.TrainingRegions)
            .NotEmpty()
            .WithMessage(IncompleteErrorMessage)
            .When(s =>
            s.LocationInformation.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation) &&
            s.LocationInformation.HasNationalDeliveryOption == "No");
    }
}
