using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class AddProviderLocationDetailsController : ControllerBase
    {
        public const string ViewPath = "~/Views/AddTrainingLocation/AddTrainingLocationDetails.cshtml";
        public const string LocationNameNotAvailable = "A location with this name already exists";
        private readonly ILogger<AddProviderLocationDetailsController> _logger;
        private readonly IMediator _mediator;

        public AddProviderLocationDetailsController(ILogger<AddProviderLocationDetailsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("{ukprn}/add-training-location/details", Name = RouteNames.GetAddProviderLocationDetails)]
        [HttpGet]
        public IActionResult GetLocationDetails()
        {
            var addressItem = GetAddressFromTempData(true);
            
            if (addressItem == null) return RedirectToRouteWithUkprn(RouteNames.GetProviderLocations);

            var model = GetViewModel(addressItem);
            return View(ViewPath, model);
        }

        [Route("{ukprn}/add-training-location/details", Name = RouteNames.PostAddProviderLocationDetails)]
        [HttpPost]
        public async Task<IActionResult> SubmitLocationDetails(ProviderLocationDetailsSubmitModel submitModel)
        {
            var addressItem = GetAddressFromTempData(true);
            if (addressItem == null) return RedirectToRouteWithUkprn(RouteNames.GetProviderLocations);

            if(ModelState.IsValid) await CheckIfNameIsAvailable(submitModel.LocationName);

            if (!ModelState.IsValid)
            {
                var model = GetViewModel(addressItem);

                model.LocationName = submitModel.LocationName;
                model.PhoneNumber = submitModel.PhoneNumber;
                model.Website = submitModel.Website;
                model.EmailAddress = submitModel.EmailAddress;

                return View(ViewPath, model);
            }

            var command = GetCommand(submitModel, addressItem);

            TempData.Remove(TempDataKeys.SelectedAddressTempDataKey);

            await _mediator.Send(command);

            return RedirectToRouteWithUkprn(RouteNames.GetProviderLocations);
        }

        private CreateProviderLocationCommand GetCommand(ProviderLocationDetailsSubmitModel submitModel, AddressItem addressItem)
        {
            var command = new CreateProviderLocationCommand()
            {
                Ukprn = base.Ukprn,
                UserId = base.UserId,
                UserDisplayName = base.UserDisplayName,
                LocationName = submitModel.LocationName,
                AddressLine1 = addressItem.AddressLine1,
                AddressLine2 = addressItem.AddressLine2,
                Town = addressItem.Town,
                Postcode = addressItem.Postcode,
                County = addressItem.County,
                Latitude = addressItem.Latitude,
                Longitude = addressItem.Longitude,
                Email = submitModel.EmailAddress,
                Website = submitModel.Website,
                Phone = submitModel.PhoneNumber
            };
            return command;
        }

        private ProviderLocationDetailsViewModel GetViewModel(AddressItem addressItem)
        {
            var model = new ProviderLocationDetailsViewModel(addressItem);
            model.CancelLink = GetUrlWithUkprn(RouteNames.GetProviderLocations);
            model.BackLink = GetUrlWithUkprn(RouteNames.GetProviderLocationAddress);
            return model;
        }

        private AddressItem GetAddressFromTempData(bool keepTempData)
        {
            TempData.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out var address);
            if (address == null)
            {
                _logger.LogWarning("Selected address not found in the Temp Data, navigating user back to the locations list page");
                return null; 
            }
            if(keepTempData) TempData.Keep(TempDataKeys.SelectedAddressTempDataKey);
            return JsonSerializer.Deserialize<AddressItem>(address.ToString());
        }

        private async Task CheckIfNameIsAvailable(string locationName)
        {
            var locations = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));
            if (locations.ProviderLocations.Any(l => l.LocationName.ToLower() == locationName.Trim().ToLower()))
            {
                ModelState.AddModelError("LocationName", LocationNameNotAvailable);
            }
        }
    }
}
