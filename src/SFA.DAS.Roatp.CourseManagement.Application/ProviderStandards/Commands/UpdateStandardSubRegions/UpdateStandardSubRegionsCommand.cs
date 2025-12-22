using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions
{
    public class UpdateStandardSubRegionsCommand : IRequest<Unit>
    {
        public int Ukprn { get; set; }
        public string LarsCode { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public List<int> SelectedSubRegions { get; set; }
    }
}
