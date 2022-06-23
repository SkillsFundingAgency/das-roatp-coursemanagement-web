using MediatR;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Application.Regions.Commands.UpdateSubRegions
{
    public class UpdateSubRegionsCommand : IRequest
    {
        public int Ukprn{ get; set; }
        public int LarsCode { get; set; }
        public string UserId { get; set; }
        public string[] SelectedSubRegions { get; set; }
    }
}
