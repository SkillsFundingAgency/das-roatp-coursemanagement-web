using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAddresses;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize( Policy = nameof(PolicyNames.HasProviderAccount))]
    public class AddressController : ControllerBase
    {
        public const string ViewPath = "~/Views/AddTrainingLocation/Address.cshtml";
        private readonly ISessionService _sessionService;
        private readonly ILogger<AddressController> _logger;
        private readonly IMediator _mediator;

        public AddressController(IMediator mediator, ILogger<AddressController> logger, ISessionService sessionService)
        {
            _mediator = mediator;
            _logger = logger;
            _sessionService = sessionService;
        }

        [Route("{ukprn}/add-training-location/address", Name = RouteNames.GetProviderLocationAddress)]
        [HttpGet]
        public async Task<IActionResult> SelectAddress()
        {
            var postcode = GetPostcodeFromSession();
            if (postcode == null) return RedirectToRouteWithUkprn(RouteNames.GetTrainingLocationPostcode);

            var model = await GetAddressViewModel(postcode);

            return View(ViewPath, model);
        }

        [Route("{ukprn}/add-training-location/address", Name = RouteNames.PostProviderLocationAddress)]
        [HttpPost]
        public async Task<IActionResult> SubmitAddress([FromForm] AddressSubmitModel model)
        {
            var postcode = GetPostcodeFromSession();
            if (postcode == null) return RedirectToRouteWithUkprn(RouteNames.GetTrainingLocationPostcode);

            if (!ModelState.IsValid) 
            {
                var viewModel = await GetAddressViewModel(postcode);
                return View(ViewPath, viewModel);
            }

            var addresses = await GetAddresses(postcode);

            var selectedAddress = addresses.Addresses.FirstOrDefault(a => a.Uprn == model.SelectedAddressUprn);
            if (selectedAddress == null)
            {
                _logger.LogError($"Get address for postcode: {postcode}, did not return selected address with Uprn: {model.SelectedAddressUprn}");
                return new StatusCodeResult(500);
            }

            TempData.Remove(TempDataKeys.SelectedAddressTempDataKey);
            TempData.Add(TempDataKeys.SelectedAddressTempDataKey, JsonSerializer.Serialize(selectedAddress));

            return RedirectToRouteWithUkprn(RouteNames.GetAddProviderLocationDetails);
        }

        private async Task<AddressViewModel> GetAddressViewModel(string postcode)
        {
            var getAddressesQueryResult = await GetAddresses(postcode);

            var model = new AddressViewModel();
            model.Postcode = postcode;
            model.ChangeLink = model.BackLink = Url.RouteUrl(RouteNames.GetTrainingLocationPostcode, new { Ukprn });
            model.CancelLink = Url.RouteUrl(RouteNames.GetProviderLocations, new { Ukprn });
            model.Addresses = getAddressesQueryResult.Addresses.Select(a => new SelectListItem { Value = a.Uprn, Text = GetDisplayName(a) }).ToList();
            return model;
        }

        private string GetDisplayName(AddressItem address) => $"{address.AddressLine1}, {address.AddressLine2}, {address.Town}";

        private string GetPostcodeFromSession()
        {
            var postcode = _sessionService.Get(SessionKeys.SelectedPostcode);
            if (string.IsNullOrEmpty(postcode))
            {
                _logger.LogInformation("Postcode not found in session, redirecting to select postcode", Ukprn, UserId);
                return null;
            }
            return postcode;
        }

        private Task<GetAddressesQueryResult> GetAddresses(string postcode) => _mediator.Send(new GetAddressesQuery(postcode));
    }
}
