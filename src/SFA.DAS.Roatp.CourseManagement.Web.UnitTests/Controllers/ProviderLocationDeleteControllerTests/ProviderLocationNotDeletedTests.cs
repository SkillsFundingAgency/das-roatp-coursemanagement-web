using System;
using System.Linq;
using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderLocationDeleteControllerTests;

[TestFixture]
public class ProviderLocationNotDeletedTests
{
    readonly string _getStandardDetailsUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void ProviderLocationNotDeleted_GetsTempDataModel_ReturnsExpectedViewModel(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        GetProviderLocationDetailsQueryResult queryResult,
        int ukprn,
        Guid id)
    {
        var tempDataMock = new Mock<ITempDataDictionary>();
        sut.TempData = tempDataMock.Object;
        sut.AddDefaultContextWithUser().AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.GetStandardDetails, _getStandardDetailsUrl);

        foreach (var standard in queryResult.ProviderLocation.Standards)
        {
            standard.HasOtherVenues = false;
        }

        object expectedQueryResult = JsonSerializer.Serialize(queryResult);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ProviderLocationTempDateKey, out expectedQueryResult));

        var result = sut.ProviderLocationNotDeleted(ukprn, id);

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult!.Model as ProviderLocationNotDeletedViewModel;
        model.Should().NotBeNull();
        model.LocationName.Should().Be(queryResult.ProviderLocation.LocationName);
        model.StandardsWithoutOtherVenues.Count.Should().Be(queryResult.ProviderLocation.Standards.Count);
        model.StandardsWithoutOtherVenues.First().StandardUrl.Should().Be(_getStandardDetailsUrl);
    }

    [Test, MoqAutoData]
    public void ProviderLocationNotDeleted_TempDataModelNotFound_RedirectsToPageNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        int ukprn,
        Guid id)
    {
        var tempDataMock = new Mock<ITempDataDictionary>();
        sut.TempData = tempDataMock.Object;
        sut.AddDefaultContextWithUser().AddUrlHelperMock();

        object expectedQueryResult = null;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ProviderLocationTempDateKey, out expectedQueryResult));

        var result = sut.ProviderLocationNotDeleted(ukprn, id);

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Contain("PageNotFound");
    }
}