using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation
{
    public class GetStandardInformationQuery : IRequest<GetStandardInformationQueryResult>
    {
        public string LarsCode { get; }
        public GetStandardInformationQuery(string larsCode)
        {
            LarsCode = larsCode;
        }
    }
}
