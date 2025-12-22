using System;
using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations
{
    public class DeleteProviderCourseLocationCommand : IRequest<Unit>
    {
        public int Ukprn { get; }
        public string LarsCode { get; }
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public DeleteProviderCourseLocationCommand(int ukprn, string larsCode, Guid id, string userId, string userDisplayName)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            Id = id;
            UserId = userId;
            UserDisplayName = userDisplayName;
        }
    }
}
