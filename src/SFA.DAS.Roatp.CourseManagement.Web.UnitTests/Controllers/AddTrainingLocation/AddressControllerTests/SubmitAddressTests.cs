using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAddresses;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddTrainingLocation.AddressControllerTests
{
    [TestFixture]
    public class SubmitAddressTests
    {
        [Test, MoqAutoData]
        public async Task InvalidState_ReturnsViewResult(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddressController sut,
            GetAddressesQueryResult queryResult,
            AddressSubmitModel model)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedPostcode, TestConstants.DefaultUkprn)).Returns(Guid.NewGuid().ToString());
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAddressesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
            sut.ModelState.AddModelError("key", "errorMessage");

            var result = await sut.SubmitAddress(model);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            viewResult.ViewName.Should().Be(AddressController.ViewPath);
        }

        [Test, MoqAutoData]
        public async Task PostcodeMissingInSession_ReturnsRedirectToRouteResult(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddressController sut,
            GetAddressesQueryResult queryResult,
            AddressSubmitModel model)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedPostcode, TestConstants.DefaultUkprn)).Returns(string.Empty);
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAddressesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

            var response = await sut.SubmitAddress(model);

            var result = response as RedirectToRouteResult;
            Assert.IsNotNull(result);
            result.RouteName.Should().Be(RouteNames.GetTrainingLocationPostcode);
        }

        [Test, MoqAutoData]
        public async Task UprnNotFound_ReturnsStatusCode500(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddressController sut,
            GetAddressesQueryResult queryResult,
            AddressSubmitModel model)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedPostcode, TestConstants.DefaultUkprn)).Returns(Guid.NewGuid().ToString());
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAddressesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

            var response = await sut.SubmitAddress(model);

            var result = response as StatusCodeResult;
            Assert.IsNotNull(result);
            result.StatusCode.Should().Be(500);
        }

        [Test, MoqAutoData]
        public async Task AllGood_SetsSelectedAddressInTempData(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddressController sut,
            GetAddressesQueryResult queryResult,
            AddressSubmitModel model,
            Mock<ITempDataDictionary> tempDataMock)
        {
            model.SelectedAddressUprn = queryResult.Addresses[0].Uprn;
            sut.AddDefaultContextWithUser();
            sut.TempData = tempDataMock.Object;
            sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedPostcode, TestConstants.DefaultUkprn)).Returns(Guid.NewGuid().ToString());
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAddressesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

            var response = await sut.SubmitAddress(model);

            var result = response as RedirectToRouteResult;
            Assert.IsNotNull(result);
            result.RouteName.Should().Be(RouteNames.GetTrainingLocationDetails);
            tempDataMock.Verify(t => t.Add(AddressController.SelectedAddressTempDataKey, queryResult.Addresses[0]));
        }
    }
}
