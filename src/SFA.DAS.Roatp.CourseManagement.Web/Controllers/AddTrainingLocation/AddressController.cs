using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class AddressController : ControllerBase
    {
        public const string ViewPath = "~/Views/AddTrainingLocation/SelectAddress.cshtml";

        [Route("{ukprn}/add-training-location/address", Name = RouteNames.SearchAddress)]
        [HttpGet]
        public Task<IActionResult> AddressSearch()
        {
            TempData.Remove(TempDataKeys.SelectedAddressTempDataKey);
            var model = new AddressSearchViewModel();
            return Task.FromResult<IActionResult>(View(ViewPath, model));
        }

        [Route("{ukprn}/add-training-location/address", Name = RouteNames.PostSearchAddress)]
        [HttpPost]
        public Task<IActionResult> PostAddressSearch([FromForm] AddressSearchSubmitModel submitModel)
        {
            var model = new AddressSearchViewModel();

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
}
