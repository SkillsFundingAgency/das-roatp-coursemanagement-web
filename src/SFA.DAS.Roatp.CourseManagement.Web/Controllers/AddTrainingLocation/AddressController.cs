using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAddresses;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class AddressController : ControllerBase
    {
        public const string ViewPath = "~/Views/AddTrainingLocation/Address.cshtml";
        public const string SelectedAddressTempDataKey = "SelectedAddressTempDataKey";
        private readonly ISessionService _sessionService;
        private readonly ILogger<AddressController> _logger;
        private readonly IMediator _mediator;

        public AddressController(IMediator mediator, ILogger<AddressController> logger, ISessionService sessionService)
        {
            _mediator = mediator;
            _logger = logger;
            _sessionService = sessionService;
        }

        [Route("{ukprn}/add-training-location/address", Name = RouteNames.GetTrainingLocationAddress)]
        [HttpGet]
        public async Task<IActionResult> SelectAddress()
        {
            var (isSuccess, model) = await GetAddressViewModel();

            if (!isSuccess) return RedirectToRouteWithUkprn(RouteNames.GetTrainingLocationPostcode);

            return View(ViewPath, model);
        }

        [Route("{ukprn}/add-training-location/address", Name = RouteNames.PostTrainingLocationAddress)]
        [HttpPost]
        public async Task<IActionResult> SubmitAddress([FromForm] AddressSubmitModel model)
        {
            if (!ModelState.IsValid) 
            {
                var (isSuccess, viewModel) = await GetAddressViewModel();
                if (!isSuccess) return RedirectToRouteWithUkprn(RouteNames.GetTrainingLocationPostcode);
                return View(ViewPath, viewModel);
            }

            var (gotAddress, postcode, result) = await GetAddresses();

            if (!gotAddress) return RedirectToRoute(RouteNames.GetTrainingLocationPostcode);

            var selectedAddress = result.Addresses.FirstOrDefault(a => a.Uprn == model.SelectedAddressUprn);
            if (selectedAddress == null)
            {
                _logger.LogError($"Get address for postcode: {postcode}, did not return selected address with Uprn: {model.SelectedAddressUprn}");
                return new StatusCodeResult(500);
            }

            TempData.Add(SelectedAddressTempDataKey, selectedAddress);

            return RedirectToRouteWithUkprn(RouteNames.GetTrainingLocationDetails);
        }

        private async Task<(bool, AddressViewModel)> GetAddressViewModel()
        {
            var (isSuccess, postcode, getAddressesQueryResult) = await GetAddresses();

            if(!isSuccess) return (false, null);

            var model = new AddressViewModel();
            model.Postcode = postcode;
            model.ChangeLink = model.BackLink = Url.RouteUrl(RouteNames.GetTrainingLocationPostcode, new { Ukprn });
            model.CancelLink = Url.RouteUrl(RouteNames.GetProviderLocations, new { Ukprn });
            model.Addresses = getAddressesQueryResult.Addresses.Select(a => new SelectListItem { Value = a.Uprn, Text = GetDisplayName(a) }).ToList();
            return (true, model);
        }

        private string GetDisplayName(AddressItem address) => $"{address.AddressLine1}, {address.AddressLine2}, {address.Town}";

        private async Task<(bool, string, GetAddressesQueryResult)> GetAddresses()
        {
            var postcode = _sessionService.Get(SessionKeys.SelectedPostcode, Ukprn.ToString());
            if (string.IsNullOrEmpty(postcode))
            {
                _logger.LogInformation("Postcode not found in session, redirecting to select postcode", Ukprn, UserId);
                return (false, null, null);
            }
            var queryResult = await _mediator.Send(new GetAddressesQuery(postcode), new CancellationToken());
            return (true, postcode, queryResult);
        }
    }
}
