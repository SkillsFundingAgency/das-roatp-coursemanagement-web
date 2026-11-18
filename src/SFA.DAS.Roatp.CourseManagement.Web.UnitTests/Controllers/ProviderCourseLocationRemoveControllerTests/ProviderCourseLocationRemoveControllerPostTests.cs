using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseLocationRemoveControllerTests;

[TestFixture]
public class ProviderCourseLocationRemoveControllerPostTests
{
    private const string Ukprn = "10012002";
    private Mock<IMediator> _mediatorMock;
    private ProviderCourseLocationRemoveController _sut;
    private const string verifyUrl = "http://test";

    [SetUp]
    public void Before_Each_Test()
    {
        _mediatorMock = new Mock<IMediator>();

        _sut = new ProviderCourseLocationRemoveController(_mediatorMock.Object, Mock.Of<ILogger<ProviderCourseLocationRemoveController>>());
        _sut.AddDefaultContextWithUser()
           .AddUrlHelperMock()
           .AddUrlForRoute(RouteNames.GetRemoveProviderCourseLocation, verifyUrl);
    }

    [Test, AutoData]
    public async Task WhenThereAreLocationsAvailable_ThenRemovesSelectedLocation(ProviderCourseLocationRemoveViewModel model, GetProviderCourseLocationsQueryResult queryResult)
    {
        model.IsOnlyLocation = false;
        _mediatorMock
           .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == model.LarsCode), It.IsAny<CancellationToken>()))
           .ReturnsAsync(queryResult);

        var result = await _sut.RemoveProviderCourseLocation(model);

        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteProviderCourseLocationCommand>(), It.IsAny<CancellationToken>()), Times.Once);

        var actual = (RedirectToRouteResult)result;
        Assert.NotNull(actual);
        actual.RouteName.Should().Be(RouteNames.GetProviderCourseLocations);
    }

    [Test, AutoData]
    public async Task WhenSelectedLocationIsTheOnlyLocation_ThenReturnsViewWithErrors(ProviderCourseLocationRemoveViewModel model, GetProviderCourseLocationsQueryResult queryResult)
    {
        model.IsOnlyLocation = true;
        _mediatorMock
           .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == model.LarsCode), It.IsAny<CancellationToken>()))
           .ReturnsAsync(queryResult);

        var result = await _sut.RemoveProviderCourseLocation(model);

        using (new AssertionScope())
        {
            var view = result.Should().BeOfType<ViewResult>().Subject;
            view.ViewData.ModelState.IsValid.Should().BeFalse();
            // verify the error message is present
            view.ViewData.ModelState.Should().ContainSingle(x => x.Key == "LocationName" && x.Value.Errors[0].ErrorMessage == "You cannot remove the only location for a standard.");
            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteProviderCourseLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
