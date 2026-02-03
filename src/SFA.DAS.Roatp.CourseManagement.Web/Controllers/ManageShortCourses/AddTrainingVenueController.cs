using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}")]
public class AddTrainingVenueController : ControllerBase
{
    public const string ViewPath = "~/Views/ManageShortCourses/AddTrainingVenueView.cshtml";

    [HttpGet("new/add-training-venue/lookup-address", Name = RouteNames.GetAddTrainingVenue)]
    public Task<IActionResult> LookupAddress(CourseType courseType)
    {
        TempData.Remove(TempDataKeys.SelectedAddressTempDataKey);
        var model = new AddTrainingVenueViewModel() { CourseType = courseType };
        return Task.FromResult<IActionResult>(View(ViewPath, model));
    }

    [HttpPost("new/add-training-venue/lookup-address", Name = RouteNames.PostAddTrainingVenue)]
    public Task<IActionResult> LookupAddress([FromForm] AddTrainingVenueSubmitModel submitModel, CourseType courseType)
    {
        var model = new AddTrainingVenueViewModel() { CourseType = courseType };

        if (!ModelState.IsValid)
        {
            return Task.FromResult<IActionResult>(View(ViewPath, model));
        }

        AddressItem selectedAddress = new AddressItem
        {
            AddressLine1 = submitModel.AddressLine1,
            AddressLine2 = submitModel.AddressLine2,
            Town = submitModel.Town,
            County = submitModel.County,
            Latitude = submitModel.Latitude,
            Longitude = submitModel.Longitude,
            Postcode = submitModel.Postcode
        };

        TempData.Remove(TempDataKeys.SelectedAddressTempDataKey);
        TempData.Add(TempDataKeys.SelectedAddressTempDataKey, JsonSerializer.Serialize(selectedAddress));

        return Task.FromResult<IActionResult>(RedirectToRoute(RouteNames.GetAddTrainingVenue, new { ukprn = Ukprn, courseType }));
    }
}
