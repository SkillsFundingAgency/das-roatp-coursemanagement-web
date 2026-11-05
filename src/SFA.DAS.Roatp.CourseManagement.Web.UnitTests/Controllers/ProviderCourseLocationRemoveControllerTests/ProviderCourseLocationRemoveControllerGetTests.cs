using System;
using System.Linq;
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
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseLocationRemoveControllerTests;

[TestFixture]
public class ProviderCourseLocationRemoveControllerGetTests
{
    private const string Ukprn = "10012002";
    private Mock<IMediator> _mediatorMock;
    private ProviderCourseLocationRemoveController _sut;
    private const string VerifyUrl = "http://test";

    [SetUp]
    public void Before_Each_Test()
    {
        _mediatorMock = new Mock<IMediator>();
        _sut = new ProviderCourseLocationRemoveController(_mediatorMock.Object, Mock.Of<ILogger<ProviderCourseLocationRemoveController>>());
        _sut.AddDefaultContextWithUser()
           .AddUrlHelperMock()
           .AddUrlForRoute(RouteNames.GetProviderCourseLocations, VerifyUrl);
    }

    [Test, AutoData]
    public async Task Get_ValidRequest_ReturnsView(
        GetProviderCourseLocationsQueryResult queryResult,
        string larsCode)
    {
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult);
        var location = queryResult.ProviderCourseLocations.FirstOrDefault();

        //Act
        var result = await _sut.GetProviderCourseLocation(larsCode, location.Id);

        using (new AssertionScope())
        {
            var model = result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<ProviderCourseLocationRemoveViewModel>().Subject;
            model.LocationName.Should().Be(location.LocationName);
            model.DeliveryMethod.HasBlockReleaseDeliveryOption.Should().Be(location.HasBlockReleaseDeliveryOption);
            model.DeliveryMethod.HasDayReleaseDeliveryOption.Should().Be(location.HasDayReleaseDeliveryOption);
            model.IsOnlyLocation.Should().Be(queryResult.ProviderCourseLocations.Count == 1);
        }

    }

    [Test, AutoData]
    public async Task Get_InvalidRequest_ThrowsInvalidOperationException(string larsCode, Guid id)
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProviderCourseLocationsQueryResult)null);

        Func<Task> action = () => _sut.GetProviderCourseLocation(larsCode, id);

        await action.Should().ThrowAsync<InvalidOperationException>();
    }
}
