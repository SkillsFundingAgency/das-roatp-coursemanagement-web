using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAddresses;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[Route("/locations")]
[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
public class LocationsController(IApiClient _apiClient) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAddresses([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 3)
        {
            return Ok(new List<AddressItem>());
        }

        var result = await _apiClient.Get<GetAddressesQueryResult>($"locations?query={query}");

        return Ok(result.Addresses);
    }
}
