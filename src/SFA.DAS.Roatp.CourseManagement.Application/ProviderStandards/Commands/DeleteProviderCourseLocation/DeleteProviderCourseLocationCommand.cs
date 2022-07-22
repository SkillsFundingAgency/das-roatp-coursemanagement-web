using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations
{
    public class DeleteProviderCourseLocationCommand : IRequest
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public int Id { get; set; }
        public string UserId { get; set; }

        public DeleteProviderCourseLocationCommand(int ukprn, int larsCode, int id, string userId)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            Id = id;
            UserId = userId;
        }
    }
}
