﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using System.Text.Json;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class AddProviderLocationDetailsController : ControllerBase
    {
        public const string ViewPath = "~/Views/AddTrainingLocation/AddTrainingLocationDetails.cshtml";
        private readonly ILogger<AddProviderLocationDetailsController> _logger;

        public AddProviderLocationDetailsController(ILogger<AddProviderLocationDetailsController> logger)
        {
            _logger = logger;
        }

        [Route("{ukprn}/add-training-location/details", Name = RouteNames.GetProviderLocationDetails)]
        [HttpGet]
        public IActionResult GetLocationDetails()
        {
            var (isSuccess, model) = GetViewModel();
            if (!isSuccess) return RedirectToRouteWithUkprn(RouteNames.GetProviderLocations);
            return View(ViewPath, model);
        }

        private (bool, ProviderLocationDetailsViewModel) GetViewModel()
        {
            TempData.TryGetValue(AddressController.SelectedAddressTempDataKey, out var address);
            if (address == null)
            {
                _logger.LogWarning("Selected address not found in the Temp Data, navigating user back to the locations list page");
                return (false, null);
            }

            var addressItem = JsonSerializer.Deserialize<AddressItem>(address.ToString());

            var model = new ProviderLocationDetailsViewModel(addressItem);
            model.CancelLink = GetUrlWithUkprn(RouteNames.GetProviderLocations);
            model.BackLink = GetUrlWithUkprn(RouteNames.GetTrainingLocationAddress);

            return (true, model);
        }
    }
}