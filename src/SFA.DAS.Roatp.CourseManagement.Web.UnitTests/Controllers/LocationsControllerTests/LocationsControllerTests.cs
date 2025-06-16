using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAddresses;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.LocationsControllerTests;

[TestFixture]
public class LocationsControllerTests
{
    [Test]
    [MoqInlineAutoData("query", 1)]
    [MoqInlineAutoData("abc", 1)]
    [MoqInlineAutoData("ab c", 1)]
    [MoqInlineAutoData("ab", 0)]
    [MoqInlineAutoData("a", 0)]
    [MoqInlineAutoData(" ab   ", 0)]
    [MoqInlineAutoData("     ab", 0)]
    [MoqInlineAutoData("ab      ", 0)]
    public void GetAddresses_ReturnsApiResponse(
        string query,
        int numberOfOuterCalls,
        [Frozen] Mock<IApiClient> outerAPiMock,
        [Greedy] LocationsController sut,
        GetAddressesQueryResult expectedResult)
    {
        var queryString = $"locations?query={query}";

        outerAPiMock.Setup(o => o.Get<GetAddressesQueryResult>(queryString)).ReturnsAsync(expectedResult);

        var actualResult = sut.GetAddresses(query);

        actualResult.Result.As<OkObjectResult>().Value.Should()
            .BeEquivalentTo(numberOfOuterCalls == 0
                ? new List<AddressItem>()
                : expectedResult.Addresses);

        outerAPiMock.Verify(o => o.Get<GetAddressesQueryResult>(It.IsAny<string>()), Times.Exactly(numberOfOuterCalls));
    }
}