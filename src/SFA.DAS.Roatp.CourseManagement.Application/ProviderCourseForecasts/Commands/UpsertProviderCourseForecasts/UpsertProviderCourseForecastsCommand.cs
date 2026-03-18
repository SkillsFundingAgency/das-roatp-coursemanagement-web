using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;

public record UpsertProviderCourseForecastsCommand(int Ukprn, string LarsCode, IEnumerable<UpsertForecastModel> Forecasts) : IRequest;
