using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Text.Json;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddTrainingLocation.AddTrainingLocationDetailsControllerTests
{
    [TestFixture]
    public class GetProviderLocationDetailsTests
    {
        [Test, MoqAutoData]
        public void GetLocationDetails_AddressInTempData_ReturnsViewResult(
            Mock<ITempDataDictionary> tempDataMock, 
            [Greedy] AddProviderLocationDetailsController sut,
            AddressItem addressItem,
            string getProviderLocationsUrl,
            string getProviderLocationAddressUrl)
        {
            sut.AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderLocations, getProviderLocationsUrl)
                .AddUrlForRoute(RouteNames.GetProviderLocationAddress, getProviderLocationAddressUrl);
            sut.TempData = tempDataMock.Object;
            object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
            tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out serialisedAddressItem));

            var result = sut.GetLocationDetails() as ViewResult;

            Assert.IsNotNull(result);
            result.ViewName.Should().Be(AddProviderLocationDetailsController.ViewPath);
            var model = result.Model as ProviderLocationDetailsViewModel;
            model.AddressLine1.Should().Be(addressItem.AddressLine1);
            model.CancelLink.Should().Be(getProviderLocationsUrl);
            model.BackLink.Should().Be(getProviderLocationAddressUrl);
        }

        [Test, MoqAutoData]
        public void GetLocationDetails_AddressNotInTempData_ReturnsRedirectResult(
            Mock<ITempDataDictionary> tempDataMock,
            [Greedy] AddProviderLocationDetailsController sut)
        {
            object address = null;
            sut.AddDefaultContextWithUser();
            sut.TempData = tempDataMock.Object;
            tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out address));

            var result = sut.GetLocationDetails() as RedirectToRouteResult;

            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetProviderLocations);
        }
    }
}
