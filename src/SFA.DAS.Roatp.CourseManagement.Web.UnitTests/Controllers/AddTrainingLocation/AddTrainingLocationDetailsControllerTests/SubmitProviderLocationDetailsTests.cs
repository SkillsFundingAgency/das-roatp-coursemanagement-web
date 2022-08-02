using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddTrainingLocation.AddTrainingLocationDetailsControllerTests
{
    [TestFixture]
    public class SubmitProviderLocationDetailsTests
    {
        [Test, MoqAutoData]
        public void AddressMissingInTempData_RedirectsToGetProviderLocations(
            Mock<ITempDataDictionary> tempDataMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] AddProviderLocationDetailsController sut,
            ProviderLocationDetailsSubmitModel model)
        {
            object address = null;
            sut.AddDefaultContextWithUser();
            sut.TempData = tempDataMock.Object;
            tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out address));

            var result = sut.SubmitLocationDetails(model).Result as RedirectToRouteResult;

            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetProviderLocations);
            mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public void ModelStateIsInvalid_ReturnsViewResult(
            Mock<ITempDataDictionary> tempDataMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] AddProviderLocationDetailsController sut,
            ProviderLocationDetailsSubmitModel model,
            AddressItem addressItem,
            string getProviderLocationsUrl,
            string getProviderLocationAddressUrl)
        {
            object address = JsonSerializer.Serialize(addressItem);
            sut.AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderLocations, getProviderLocationsUrl)
                .AddUrlForRoute(RouteNames.GetProviderLocationAddress, getProviderLocationAddressUrl);
            sut.TempData = tempDataMock.Object;
            tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out address));
            sut.ModelState.AddModelError("key", "message");

            var result = sut.SubmitLocationDetails(model).Result as ViewResult;

            result.Should().NotBeNull();
            result.ViewName.Should().Be(AddProviderLocationDetailsController.ViewPath);
            var actualModel = (ProviderLocationDetailsViewModel) result.Model;
            actualModel.LocationName.Should().Be(model.LocationName);
            actualModel.EmailAddress.Should().Be(model.EmailAddress);
            actualModel.Website.Should().Be(model.Website);
            actualModel.PhoneNumber.Should().Be(model.PhoneNumber);
            actualModel.CancelLink.Should().Be(getProviderLocationsUrl);
            actualModel.BackLink.Should().Be(getProviderLocationAddressUrl);
            mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, MoqAutoData]
        public void ModelStateIsValid_InvokesMediatorWithCreateCommand(
            Mock<ITempDataDictionary> tempDataMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] AddProviderLocationDetailsController sut,
            ProviderLocationDetailsSubmitModel submitModel,
            AddressItem addressItem,
            string getProviderLocationsUrl,
            string getProviderLocationAddressUrl)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetAllProviderLocationsQueryResult());
            object address = JsonSerializer.Serialize(addressItem);
            sut.AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderLocations, getProviderLocationsUrl)
                .AddUrlForRoute(RouteNames.GetProviderLocationAddress, getProviderLocationAddressUrl);
            sut.TempData = tempDataMock.Object;
            tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out address));

            var result = sut.SubmitLocationDetails(submitModel).Result as RedirectToRouteResult;

            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetProviderLocations);
            tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedAddressTempDataKey));
            mediatorMock.Verify(m => m.Send(It.Is<CreateProviderLocationCommand>(c =>
                c.Ukprn.ToString() == TestConstants.DefaultUkprn &&
                c.UserId == TestConstants.DefaultUserId &&
                c.LocationName == submitModel.LocationName &&
                c.AddressLine1 == addressItem.AddressLine1 &&
                c.AddressLine2 == addressItem.AddressLine2 &&
                c.Town == addressItem.Town &&
                c.Postcode == addressItem.Postcode &&
                c.County == addressItem.County &&
                c.Latitude == addressItem.Latitude &&
                c.Longitude == addressItem.Longitude &&
                c.Email == submitModel.EmailAddress &&
                c.Website == submitModel.Website &&
                c.Phone == submitModel.PhoneNumber
            ), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public void LocationNameIsNotDistinct_ReturnsViewResult(
            Mock<ITempDataDictionary> tempDataMock,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] AddProviderLocationDetailsController sut,
            ProviderLocationDetailsSubmitModel model,
            AddressItem addressItem,
            string getProviderLocationsUrl,
            string getProviderLocationAddressUrl,
            GetAllProviderLocationsQueryResult allLocations)
        {
            allLocations.ProviderLocations.First().LocationName = model.LocationName;
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(allLocations);
            object address = JsonSerializer.Serialize(addressItem);
            sut.AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderLocations, getProviderLocationsUrl)
                .AddUrlForRoute(RouteNames.GetProviderLocationAddress, getProviderLocationAddressUrl);
            sut.TempData = tempDataMock.Object;
            tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedAddressTempDataKey, out address));

            var result = sut.SubmitLocationDetails(model).Result as ViewResult;

            result.Should().NotBeNull();
            result.ViewName.Should().Be(AddProviderLocationDetailsController.ViewPath);
            mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            sut.ModelState.ErrorCount.Should().Be(1);
            sut.ModelState.Keys.Contains("LocationName");
        }
    }
}
