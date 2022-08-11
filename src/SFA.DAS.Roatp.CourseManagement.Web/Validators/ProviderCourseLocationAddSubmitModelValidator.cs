using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;


namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class ProviderCourseLocationAddSubmitModelValidator : AbstractValidator<ProviderCourseLocationAddSubmitModel>
    {
        public const string TrainingVenueErrorMessage = "You must choose a venue";
        public const string DeliveryMethodErrorMessage = "You must choose a delivery method";
        public ProviderCourseLocationAddSubmitModelValidator()
        {
            RuleFor(p => p.TrainingVenueNavigationId).NotEmpty()
               .WithMessage(TrainingVenueErrorMessage);

            RuleFor(x => x.HasDayReleaseDeliveryOption).Equal(true).When(a=>a.HasBlockReleaseDeliveryOption == false)
            .WithMessage(DeliveryMethodErrorMessage);
        }
    }
}
