using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddNationalLocation;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditNationalDeliveryOptionControllerTests
{
    [TestFixture]
    public class EditNationalDeliveryOptionPostTests : EditNationalDeliveryOptionControllerTestBase
    {
        [Test, AutoData]
        public async Task Post_InvalidState_ReturnsView(ConfirmNationalProviderSubmitModel model, string larsCode)
        {
            SetupController();
            Sut.ModelState.AddModelError("key", "error");

            var result = await Sut.Index(larsCode, model);

            var actual = (ViewResult)result;

            Assert.NotNull(actual);
        }

        [Test, AutoData]
        public async Task Post_HasNationalDeliveryOption_AddsNationalLocation_RedirectsToStandardDetails(ConfirmNationalProviderSubmitModel model, string larsCode)
        {
            SetupController();
            model.HasNationalDeliveryOption = true;

            var result = await Sut.Index(larsCode, model);

            var actual = (RedirectToRouteResult)result;
            Assert.NotNull(actual);
            actual.RouteName.Should().Be(RouteNames.GetStandardDetails);
            MediatorMock.Verify(m => m.Send(It.Is<DeleteCourseLocationsCommand>(c => c.DeleteProviderCourseLocationOption == Domain.ApiModels.DeleteProviderCourseLocationOption.DeleteEmployerLocations), It.IsAny<CancellationToken>()));
            MediatorMock.Verify(m => m.Send(It.IsAny<AddNationalLocationToStandardCommand>(), It.IsAny<CancellationToken>()));
        }

        [Test, AutoData]
        public async Task Post_DoesNotDeliverNationally_RedirectsToSelectRegions(ConfirmNationalProviderSubmitModel model, string larsCode)
        {
            SetupController();
            model.HasNationalDeliveryOption = false;

            var result = await Sut.Index(larsCode, model);

            var actual = (RedirectToRouteResult)result;
            Assert.NotNull(actual);
            actual.RouteName.Should().Be(RouteNames.GetStandardSubRegions);
            MediatorMock.Verify(m => m.Send(It.IsAny<AddNationalLocationToStandardCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
