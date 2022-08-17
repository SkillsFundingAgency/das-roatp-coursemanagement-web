using MediatR;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations
{
    public class DeleteProviderCourseCommand : IRequest
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public string UserId { get; set; }

        public DeleteProviderCourseCommand(int ukprn, int larsCode, string userId)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            UserId = userId;
        }
    }
}
