using MediatR;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails
{
    public class GetProviderCourseLocationsQuery : IRequest<GetProviderCourseLocationsQueryResult>
    {
        public int Ukprn { get; }
        public string LarsCode { get; }

        public GetProviderCourseLocationsQuery(int ukprn, string larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }

    public class GetProviderCourseLocationsQueryResult
    {
        public StandardLookupModel[] Locations { get; set; }
    }
}
