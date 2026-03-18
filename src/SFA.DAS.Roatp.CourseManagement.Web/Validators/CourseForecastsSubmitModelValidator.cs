using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Forecasts;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators;

public class CourseForecastsSubmitModelValidator : AbstractValidator<CourseForecastsSubmitModel>
{
    public CourseForecastsSubmitModelValidator()
    {
        RuleForEach(m => m.Forecasts).SetValidator(new ForecastModelValidator());
    }
}

public class ForecastModelValidator : AbstractValidator<ForecastSubmitModel>
{
    public ForecastModelValidator()
    {
        RuleFor(m => m.EstimatedLearners)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Estimated learners must be a positive number")
            .LessThanOrEqualTo(99999)
            .WithMessage("Estimated learners must be less than 100000");
    }
}
