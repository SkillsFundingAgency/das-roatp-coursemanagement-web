using System;
using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Forecasts;

public class CourseForecastsViewModel : CourseForecastsSubmitModel, IBackLink
{
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public string CourseDisplayName { get; set; }
    public string LastUpdatedDate { get; set; }
    public bool ShowLastUpdatedDate => !string.IsNullOrEmpty(LastUpdatedDate);
}

public class CourseForecastsSubmitModel : ShortCourseBaseViewModel
{
    public List<ForecastSubmitModel> Forecasts { get; set; }
}

public class ForecastSubmitModel
{
    public string Id => $"{TimePeriod}-{Quarter}";
    public string TimePeriod { get; set; }
    public int Quarter { get; set; }
    public string Description { get; set; }
    public int? EstimatedLearners { get; set; }

    public static implicit operator ForecastSubmitModel(ProviderCourseForecast forecast)
    {
        return new ForecastSubmitModel
        {
            TimePeriod = forecast.TimePeriod,
            Quarter = forecast.Quarter,
            Description = GetDateRange(forecast.StartDate, forecast.EndDate),
            EstimatedLearners = forecast.EstimatedLearners
        };
    }

    public static implicit operator UpsertForecastModel(ForecastSubmitModel source)
        => new(source.TimePeriod, source.Quarter, source.EstimatedLearners);

    internal static string GetDateRange(DateTime startDate, DateTime endDate)
    {
        return $"{startDate.ToString("MMM")} {startDate.Day.ToNumberWithSuffix()} to {endDate.ToString("MMM")} {endDate.Day.ToNumberWithSuffix()} {endDate.ToString("yyyy")}";
    }
}
