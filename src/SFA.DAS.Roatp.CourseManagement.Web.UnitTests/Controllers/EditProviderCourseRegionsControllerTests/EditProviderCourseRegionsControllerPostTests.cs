using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllStandardRegions;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditProviderCourseRegionsControllerTests
{
    [TestFixture]
    public class EditProviderCourseRegionsControllerPostTests
    {
        private static string DetailsUrl = Guid.NewGuid().ToString();
        private Mock<ILogger<EditProviderCourseRegionsController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private EditProviderCourseRegionsController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<EditProviderCourseRegionsController>>();
            _mediatorMock = new Mock<IMediator>();

            _sut = new EditProviderCourseRegionsController(_mediatorMock.Object, _loggerMock.Object);
            _sut
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetStandardDetails, DetailsUrl);
        }

        [Test, AutoData]
        public async Task Post_ValidModel_SendsUpdateCommand(RegionsSubmitModel model, int larsCode)
        {
            string[] selectedSubRegions = new string[] { "1", "2" };
            model.SelectedSubRegions = selectedSubRegions;

            var result = await _sut.UpdateStandardSubRegions(larsCode, model);
            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateStandardSubRegionsCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            var routeResult = result as RedirectToRouteResult;
            routeResult.Should().NotBeNull();
            routeResult.RouteName.Should().Be(RouteNames.GetStandardDetails);
            routeResult.RouteValues.Should().NotBeEmpty().And.HaveCount(2);
            routeResult.RouteValues.Should().ContainKey("ukprn").WhoseValue.Should().Be(Convert.ToInt32(TestConstants.DefaultUkprn));
            routeResult.RouteValues.Should().ContainKey("larsCode").WhoseValue.Should().Be(larsCode);
        }

        [Test, AutoData]
        public async Task Post_InValidData_RedirectToSameView(RegionsSubmitModel model, GetAllStandardRegionsQueryResult queryResult, int larsCode)
        {
            _sut.ModelState.AddModelError("key", "error");
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllStandardRegionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);
            model.SelectedSubRegions = null;
            var result = await _sut.UpdateStandardSubRegions(larsCode, model);
            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateStandardSubRegionsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            result.As<ViewResult>().ViewName.Should().Be(EditProviderCourseRegionsController.ViewPath);
            result.As<ViewResult>().Model.As<RegionsViewModel>().AllRegions.TrueForAll(r => r.IsSelected);
        }
    }
}
