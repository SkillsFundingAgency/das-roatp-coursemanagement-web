using MediatR;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations
{
    public class DeleteCourseLocationsCommand : IRequest<Unit>
    {
        public int Ukprn { get; }
        public string LarsCode { get; }
        public string UserId { get; }
        public string UserDisplayName { get; }
        public DeleteProviderCourseLocationOption DeleteProviderCourseLocationOption { get; }

        public DeleteCourseLocationsCommand(int ukprn, string larsCode, string userId, string userDisplayName, DeleteProviderCourseLocationOption option)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            UserId = userId;
            UserDisplayName = userDisplayName;
            DeleteProviderCourseLocationOption = option;
        }
    }
}
