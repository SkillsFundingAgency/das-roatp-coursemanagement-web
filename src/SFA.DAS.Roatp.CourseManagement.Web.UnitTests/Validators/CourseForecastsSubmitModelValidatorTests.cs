using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Forecasts;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators;

public class CourseForecastsSubmitModelValidatorTests
{
    [TestCase(-1)]
    [TestCase(100000)]
    public void ValidateForecasts_IsInvalid(int? learners)
    {
        CourseForecastsSubmitModel model = new()
        {
            Forecasts = [new ForecastSubmitModel() { EstimatedLearners = learners }]
        };
        CourseForecastsSubmitModelValidator sut = new();

        var result = sut.Validate(model);

        result.IsValid.Should().BeFalse();
    }

    [TestCase(null)]
    [TestCase(0)]
    [TestCase(1)]
    public void ValidateForecasts_IsValid(int? learners)
    {
        CourseForecastsSubmitModel model = new()
        {
            Forecasts = [new ForecastSubmitModel() { EstimatedLearners = learners }]
        };
        CourseForecastsSubmitModelValidator sut = new();

        var result = sut.Validate(model);

        result.IsValid.Should().BeTrue();
    }
}
