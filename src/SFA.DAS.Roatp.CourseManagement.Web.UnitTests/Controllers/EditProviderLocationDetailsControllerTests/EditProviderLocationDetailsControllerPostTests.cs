using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.UpdateProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditProviderLocationDetailsControllerTests
{
    [TestFixture]
    public class EditProviderLocationDetailsControllerPostTests
    {
        private const string Ukprn = "10012002";
        private Mock<IMediator> _mediatorMock;
        private EditProviderLocationDetailsController _sut;
        string verifyUrl = "http://test";

        [SetUp]
        public void Before_Each_Test()
        {
            _mediatorMock = new Mock<IMediator>();
            _sut = new EditProviderLocationDetailsController(_mediatorMock.Object, Mock.Of<ILogger<EditProviderLocationDetailsController>>());
            _sut.AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderLocations, verifyUrl);
        }

        [Test, AutoData]
        public async Task UpdateProviderLocationDetails_ValidRequest_ReturnsView(
            GetProviderLocationDetailsQueryResult queryResult,
            GetAllProviderLocationsQueryResult allProviderLocationsQueryResult,
            ProviderLocationDetailsSubmitModel model,
            Guid id)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(allProviderLocationsQueryResult);


            var result = await _sut.UpdateProviderLocationDetails(model, id);

            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateProviderLocationDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            var actual = (RedirectToRouteResult)result;
            Assert.NotNull(actual);
            actual.RouteName.Should().Be(RouteNames.GetProviderLocationDetails);
        }

        [Test, AutoData]
        public async Task UpdateProviderLocationDetails_InvalidRequest_ReturnsSameView(
            GetProviderLocationDetailsQueryResult queryResult,
            ProviderLocationDetailsSubmitModel model,
            Guid id,
            GetAllProviderLocationsQueryResult allProviderLocationsQueryResult)
        {
            _sut.ModelState.AddModelError("key", "errorMessage");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderLocationDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(allProviderLocationsQueryResult);

            var result = await _sut.UpdateProviderLocationDetails(model, id);

            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateProviderLocationDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Never);


            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("EditProviderLocationsDetails.cshtml");
            var viewmodel = viewResult.Model as ProviderLocationViewModel;
            viewmodel.Should().NotBeNull();
            viewmodel!.TrainingVenuesUrl.Should().Be(verifyUrl);
        }

        [Test, AutoData]
        public async Task UpdateProviderLocationDetails_LocationNameIsNotDistinct_ReturnsSameView(
            GetProviderLocationDetailsQueryResult queryResult,
            ProviderLocationDetailsSubmitModel model,
            Guid id,
            GetAllProviderLocationsQueryResult allProviderLocationsQueryResult)
        {
            allProviderLocationsQueryResult.ProviderLocations.First().LocationName = model.LocationName;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderLocationDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(allProviderLocationsQueryResult);

            var result = await _sut.UpdateProviderLocationDetails(model, id);

            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateProviderLocationDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Never);


            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("EditProviderLocationsDetails.cshtml");
            var viewmodel = viewResult.Model as ProviderLocationViewModel;
            viewmodel.Should().NotBeNull();
            viewmodel!.TrainingVenuesUrl.Should().Be(verifyUrl);
        }
    }
}
