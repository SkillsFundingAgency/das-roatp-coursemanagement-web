using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;

public record GetProviderCourseForecastsQueryResult(string LarsCode, string CourseName, int CourseLevel, IEnumerable<ProviderCourseForecast> Forecasts);

public class ProviderCourseForecast
{
    public string TimePeriod { get; set; }
    public int Quarter { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? EstimatedLearners { get; set; }
    public DateTime? UpdatedDate { get; set; }
}