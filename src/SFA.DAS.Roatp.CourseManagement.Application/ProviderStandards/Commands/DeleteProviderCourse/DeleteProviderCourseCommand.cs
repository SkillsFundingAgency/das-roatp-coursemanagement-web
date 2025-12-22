using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations
{
    public class DeleteProviderCourseCommand : IRequest<Unit>
    {
        public int Ukprn { get; }
        public string LarsCode { get; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public DeleteProviderCourseCommand(int ukprn, string larsCode, string userId, string userDisplayName)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            UserId = userId;
            UserDisplayName = userDisplayName;
        }
    }
}
