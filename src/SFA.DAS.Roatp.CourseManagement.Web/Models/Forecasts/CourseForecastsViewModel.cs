using System;
using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Forecasts;

public class CourseForecastsViewModel : CourseForecastsSubmitModel, IBackLink
{
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public string CourseDisplayName { get; set; } = "Digital marketing apprenticeship unit (level 2)";
    public string LastUpdatedDate { get; set; }
    public bool ShowLastUpdatedDate => !string.IsNullOrEmpty(LastUpdatedDate);
}

public class CourseForecastsSubmitModel : ShortCourseBaseViewModel
{
    public List<ForecastModel> Forecasts { get; set; }
}

public class ForecastModel
{
    public string Id => $"{TimePeriod}-{Quarter}";
    public string TimePeriod { get; set; }
    public int Quarter { get; set; }
    public string DisplayName { get; set; }
    public int? EstimatedLearners { get; set; }

    public static implicit operator ForecastModel(ProviderCourseForecast forecast)
    {
        return new ForecastModel
        {
            TimePeriod = forecast.TimePeriod,
            Quarter = forecast.Quarter,
            DisplayName = GetDateRange(forecast.StartDate, forecast.EndDate),
            EstimatedLearners = forecast.EstimatedLearners
        };
    }

    internal static string GetDateRange(DateTime startDate, DateTime endDate)
    {
        return $"{startDate.ToString("MMM")} {startDate.Day.ToNumberWithSuffix()} to {endDate.ToString("MMM")} {endDate.Day.ToNumberWithSuffix()} {endDate.ToString("yyyy")}";
    }
}
