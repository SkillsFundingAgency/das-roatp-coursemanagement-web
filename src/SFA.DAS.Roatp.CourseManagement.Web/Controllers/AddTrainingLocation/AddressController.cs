using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation;

[Route("{ukprn}/add-training-location/address")]
public class AddressController(IValidator<AddressSearchSubmitModel> _validator) : ControllerBase
{
    public const string ViewPath = "~/Views/AddTrainingLocation/SelectAddress.cshtml";

    [HttpGet(Name = RouteNames.SearchAddress)]
    public Task<IActionResult> AddressSearch()
    {
        TempData.Remove(TempDataKeys.SelectedAddressTempDataKey);
        var model = new AddressSearchViewModel();
        return Task.FromResult<IActionResult>(View(ViewPath, model));
    }

    [HttpPost(Name = RouteNames.PostSearchAddress)]
    public Task<IActionResult> PostAddressSearch([FromForm] AddressSearchSubmitModel submitModel)
    {
        var model = new AddressSearchViewModel();

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

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

        return Task.FromResult<IActionResult>(RedirectToRouteWithUkprn(RouteNames.GetAddProviderLocationDetails));
    }
}
