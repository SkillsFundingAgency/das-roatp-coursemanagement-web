using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAddresses;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddTrainingLocation.AddressControllerTests
{
    [TestFixture]
    public class SelectAddressTests
    {
        private const string Postcode = "CV12WT";
        private static string GetTrainingLocationPostcodeLink = Guid.NewGuid().ToString();
        private static string GetProviderLocationsLink = Guid.NewGuid().ToString();
        private readonly AddressItem _addressItem = new AddressItem { Uprn = Guid.NewGuid().ToString(), AddressLine1 = "add line1", AddressLine2 = "add line2", Town = "Town" };
        private Mock<IMediator> _mediatorMock;
        private Mock<ISessionService> _sessionServiceMock;
        private AddressController _sut;
        private List<AddressItem> _addresses;
        private ViewResult _result;
        private AddressViewModel _model;

        [SetUp]
        public void OnSelectAddress()
        {
            _addresses = new List<AddressItem>
            {
                _addressItem
            };

            _mediatorMock = new Mock<IMediator>();
            _mediatorMock.Setup(m => m.Send(It.Is<GetAddressesQuery>(q => q.Postcode == Postcode), It.IsAny<CancellationToken>())).ReturnsAsync(new GetAddressesQueryResult { Addresses = _addresses });

            _sessionServiceMock = new Mock<ISessionService>();
            _sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedPostcode)).Returns(Postcode);
            _sut = new AddressController(_mediatorMock.Object, Mock.Of<ILogger<AddressController>>(), _sessionServiceMock.Object);
            _sut
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetTrainingLocationPostcode, GetTrainingLocationPostcodeLink)
                .AddUrlForRoute(RouteNames.GetProviderLocations, GetProviderLocationsLink);

            _result = _sut.SelectAddress().Result as ViewResult;
            _model = _result.Model as AddressViewModel;
        }

        [Test]
        public void GetPostcodeFromSession()
        {
            _sessionServiceMock.Verify(s => s.Get(SessionKeys.SelectedPostcode));
        }

        [Test]
        public void GetAddressesFromOuterApi()
        {
            _mediatorMock.Verify(m => m.Send(It.Is<GetAddressesQuery>(q => q.Postcode == Postcode), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void ReturnsViewResult()
        {
            _result.Should().NotBeNull();
            _result.ViewName.Should().Be(AddressController.ViewPath);
            _model.Should().NotBeNull();
        }

        [Test]
        public void ModelHasCorrectPostcode()
        {
            _model.Postcode.Should().Be(Postcode);
        }

        [Test]
        public void ModelHasCorrectBackLink()
        {
            _model.BackLink.Should().Be(GetTrainingLocationPostcodeLink);
        }

        [Test]
        public void ModelHasCorrectChangeLink()
        {
            _model.ChangeLink.Should().Be(GetTrainingLocationPostcodeLink);
        }

        [Test]
        public void ModelHasCorrectCancelLink()
        {
            _model.CancelLink.Should().Be(GetProviderLocationsLink);
        }

        [Test]
        public void ModelHasTransformedAddressList()
        {
            _model.Addresses.Count.Should().Be(1);
            _model.Addresses[0].Text.Should().Be($"{_addressItem.AddressLine1}, {_addressItem.AddressLine2}, {_addressItem.Town}");
            _model.Addresses[0].Value.Should().Be(_addressItem.Uprn);
        }

        [Test, MoqAutoData]
        public async Task PostcodeNotInSession_RedirectsToGetPostcode(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] AddressController sut)
        {
            sut.AddDefaultContextWithUser();
            sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedPostcode)).Returns(string.Empty);

            var response = await sut.SelectAddress();

            var result = (RedirectToRouteResult)response;
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetTrainingLocationPostcode);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetAddressesQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

    }
}
