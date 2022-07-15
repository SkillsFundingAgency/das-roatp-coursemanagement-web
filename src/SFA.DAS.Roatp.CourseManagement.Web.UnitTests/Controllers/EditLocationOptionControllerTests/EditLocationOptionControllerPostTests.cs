using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditLocationOptionControllerTests
{
    [TestFixture]
    public class EditLocationOptionControllerPostTests : EditLocationOptionControllerTestBase
    {
        [Test]
        public async Task Post_InvalidModel_ReturnsViewWithValidationError()
        {
            var model = new EditLocationOptionViewModel() { LocationOption = LocationOption.None };
            _sut.ModelState.AddModelError("key", "message");

            var response = await _sut.Index(123, model);

            var result = (ViewResult)response;

            result.Model.Should().BeEquivalentTo(model);
            _mediatorMock.Verify(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, AutoData]
        public async Task Post_LocationOptionProvider_DeletesEmployerLocations(EditLocationOptionViewModel model)
        {
            model.LocationOption = LocationOption.ProviderLocation;

            await _sut.Index(123, model);

            _mediatorMock.Verify(m => m.Send(
                It.Is<DeleteCourseLocationsCommand>(d => d.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteEmployerLocations), It.IsAny<CancellationToken>()));
        }

        [Test, AutoData]
        public async Task Post_LocationOptionEmployer_DeletesProviderLocations(EditLocationOptionViewModel model)
        {
            model.LocationOption = LocationOption.EmployerLocation;

            await _sut.Index(123, model);

            _mediatorMock.Verify(m => m.Send(
                It.Is<DeleteCourseLocationsCommand>(d => d.DeleteProviderCourseLocationOption == DeleteProviderCourseLocationOption.DeleteProviderLocations), It.IsAny<CancellationToken>()));
        }

        [Test, AutoData]
        public async Task Post_LocationOptionBoth_DoesNotDeleteAnyProviderLocations(EditLocationOptionViewModel model)
        {
            model.LocationOption = LocationOption.Both;

            await _sut.Index(123, model);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<DeleteCourseLocationsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test, AutoData]
        public async Task Post_LocationOptionProvider_SetLocationOptionInSession(EditLocationOptionViewModel model)
        {
            model.LocationOption = LocationOption.ProviderLocation;

            await _sut.Index(123, model);

            _sessionServiceMock.Verify(s => s.Set(SessionKeys.SelectedLocationOption, model.LocationOption.ToString()));
        }

        [Test, AutoData]
        public async Task Post_LocationOptionProvider_RedirectToProviderCourseLocations(EditLocationOptionViewModel model)
        {
            model.LocationOption = LocationOption.ProviderLocation;

            var result = await _sut.Index(123, model);

            var routeResult = result as RedirectToRouteResult;
            routeResult.Should().NotBeNull();
            routeResult.RouteName.Should().Be(RouteNames.GetProviderCourseLocations);
        }
    }
}
