namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;

public record UpsertForecastModel(string TimePeriod, int Quarter, int? EstimatedLearners);
