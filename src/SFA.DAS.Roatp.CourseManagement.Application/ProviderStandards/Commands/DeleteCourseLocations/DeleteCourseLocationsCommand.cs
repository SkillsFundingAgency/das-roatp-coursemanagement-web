using MediatR;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations
{
    public class DeleteCourseLocationsCommand : IRequest
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public string UserId { get; }
        public DeleteProviderCourseLocationOption DeleteProviderCourseLocationOption { get; }

        public DeleteCourseLocationsCommand(int ukprn, int larsCode, string userId, DeleteProviderCourseLocationOption option)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            UserId = userId;
            DeleteProviderCourseLocationOption = option;
        }
    }
}
