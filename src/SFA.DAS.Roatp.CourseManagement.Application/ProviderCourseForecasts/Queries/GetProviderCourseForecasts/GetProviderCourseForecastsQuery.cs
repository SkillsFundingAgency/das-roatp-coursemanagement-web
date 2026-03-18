using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;

public record GetProviderCourseForecastsQuery(int Ukprn, string LarsCode) : IRequest<GetProviderCourseForecastsQueryResult>;
