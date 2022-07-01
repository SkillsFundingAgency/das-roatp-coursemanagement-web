using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.Regions.Commands.UpdateStandardSubRegions
{
    public class UpdateStandardSubRegionsCommand : IRequest
    {
        public int Ukprn{ get; set; }
        public int LarsCode { get; set; }
        public string UserId { get; set; }
        public List<int> SelectedSubRegions { get; set; }
    }
}
