using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard
{
    public class CourseLocationAddSubmitModelValidator : AbstractValidator<CourseLocationAddSubmitModel>
    {
        public const string TrainingVenueErrorMessage = "You must select a training venue";
        public const string DeliveryMethodErrorMessage = "You must select a delivery method";
        public CourseLocationAddSubmitModelValidator()
        {
            RuleFor(p => p.TrainingVenueNavigationId).NotEmpty()
                .WithMessage(TrainingVenueErrorMessage);

            RuleFor(x => x.HasDayReleaseDeliveryOption).Equal(true).When(a => !a.HasBlockReleaseDeliveryOption)
                .WithMessage(DeliveryMethodErrorMessage);
        }
    }
}
