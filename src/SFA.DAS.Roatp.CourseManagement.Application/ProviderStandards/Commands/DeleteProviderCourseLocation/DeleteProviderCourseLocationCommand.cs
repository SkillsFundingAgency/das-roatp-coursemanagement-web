using MediatR;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations
{
    public class DeleteProviderCourseLocationCommand : IRequest
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public DeleteProviderCourseLocationCommand(int ukprn, int larsCode, Guid id, string userId, string userDisplayName)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            Id = id;
            UserId = userId;
            UserDisplayName = userDisplayName;
        }
    }
}
