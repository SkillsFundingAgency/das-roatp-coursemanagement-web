using MediatR;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;

public record GetProviderAvailableCourseDetailsQuery(int Ukprn, CourseType CourseType, string LarsCode) : IRequest<GetProviderAvailableCourseDetailsQueryResult>;