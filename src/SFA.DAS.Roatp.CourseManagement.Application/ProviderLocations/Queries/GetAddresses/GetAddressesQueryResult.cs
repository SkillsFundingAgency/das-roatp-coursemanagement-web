using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAddresses
{
    public class GetAddressesQueryResult
    {
        public List<AddressItem> Addresses { get; set; }
    }
}
