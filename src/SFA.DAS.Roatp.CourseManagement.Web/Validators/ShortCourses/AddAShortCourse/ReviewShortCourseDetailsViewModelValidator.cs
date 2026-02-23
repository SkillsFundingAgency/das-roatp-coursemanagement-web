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
            !string.IsNullOrWhiteSpace(s.ContactUsEmail) &&
            !string.IsNullOrWhiteSpace(s.ContactUsPhoneNumber) &&
            !string.IsNullOrWhiteSpace(s.StandardInfoUrl))
            .WithMessage(s => $"Enter all contact details for this {s.ApprenticeshipTypeLower}");

        RuleFor(s => s.DeliveryLocations)
            .NotEmpty()
            .WithMessage(s => $"Select training options for this {s.ApprenticeshipTypeLower}");

        RuleFor(s => s.TrainingVenues)
            .NotEmpty()
            .WithMessage(IncompleteErrorMessage)
            .When(s =>
            s.LocationOptions.Contains(ShortCourseLocationOption.ProviderLocation));

        RuleFor(s => s.HasNationalDeliveryOption)
            .NotNull()
            .WithMessage(IncompleteErrorMessage)
            .When(s =>
            s.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation));

        RuleFor(s => s.TrainingRegions)
            .NotEmpty()
            .WithMessage(IncompleteErrorMessage)
            .When(s =>
            s.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation) &&
            s.HasNationalDeliveryOption == "No");
    }
}
