using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddTrainingLocation.AddressControllerTests;

[TestFixture]
public class PostAddressSearchTests
{

    [Test, MoqAutoData]
    public void InvalidStatus_ReturnsViewResult(
        [Greedy] AddressController sut,
        AddressSearchSubmitModel model)
    {

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        var result = sut.PostAddressSearch(model);

        var viewResult = result.Result as ViewResult;
        Assert.IsNotNull(viewResult);
        viewResult.ViewName.Should().Be(AddressController.ViewPath);
    }

    [Test, MoqAutoData]
    public void Valid_SetsSelectedAddressInTempData(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddressController sut,
        AddressSearchSubmitModel submitModel,
        Mock<ITempDataDictionary> tempDataMock)
    {
        AddressItem selectedAddress = new AddressItem
        {
            AddressLine1 = submitModel.AddressLine1,
            AddressLine2 = submitModel.AddressLine2,
            Town = submitModel.Town,
            County = submitModel.County,
            Latitude = submitModel.Latitude,
            Longitude = submitModel.Longitude,
            Postcode = submitModel.Postcode,
            Uprn = null
        };

        var expectedValueInTempData = JsonSerializer.Serialize(selectedAddress);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;

        var response = sut.PostAddressSearch(submitModel);

        var result = response.Result as RedirectToRouteResult;
        Assert.IsNotNull(result);
        result.RouteName.Should().Be(RouteNames.GetAddProviderLocationDetails);
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedAddressTempDataKey));
        tempDataMock.Verify(t => t.Add(TempDataKeys.SelectedAddressTempDataKey, expectedValueInTempData));
    }
}
