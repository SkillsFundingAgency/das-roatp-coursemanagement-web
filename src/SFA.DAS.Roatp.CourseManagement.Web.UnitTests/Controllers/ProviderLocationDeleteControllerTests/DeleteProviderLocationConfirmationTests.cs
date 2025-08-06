using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;


namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderLocationDeleteControllerTests;

[TestFixture]

public class DeleteProviderLocationConfirmationTests
{
    [Test, MoqAutoData]
    public async Task Get_ValidRequest_NoOrphanedStandards_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        GetProviderLocationDetailsQueryResult queryResult)
    {
        var ukprn = 12345678;
        var id = Guid.NewGuid();
        sut.AddDefaultContextWithUser().AddUrlHelperMock();

        foreach (var standard in queryResult.ProviderLocation.Standards)
        {
            standard.HasOtherVenues = true;
        }

        var source = queryResult.ProviderLocation;

        var expectedStandards = source.Standards.Select(s => (ProviderLocationStandardModel)s)
            .OrderBy(s => s.CourseDisplayName).ToList();

        mediatorMock
            .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(r => r.Ukprn == ukprn && r.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult);

        var result = await sut.DeleteProviderLocationConfirmation(ukprn, id);

        mediatorMock.Verify(m => m.Send(It.IsAny<GetProviderLocationDetailsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult!.Model as ProviderLocationConfirmDeleteViewModel;
        model.Should().NotBeNull();
        model.NavigationId.Should().Be(queryResult.ProviderLocation.NavigationId);
        model.Standards.Should().BeEquivalentTo(expectedStandards);
        model.BackUrl.Should().BeNull();
    }

    [Test]
    [MoqInlineAutoData(0, true, false, false)]
    [MoqInlineAutoData(1, false, true, false)]
    [MoqInlineAutoData(2, false, false, true)]
    [MoqInlineAutoData(3, false, false, true)]
    public async Task Get_ValidRequest_NoOrphanedStandards_ReturnsExpectedValuesInModel(
        int numberOfStandards,
        bool hasNoStandards,
        bool hasOneStandard,
        bool hasMultipleStandards,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        GetProviderLocationDetailsQueryResult queryResult)
    {
        var ukprn = 12345678;
        var id = Guid.NewGuid();

        sut.AddDefaultContextWithUser();

        var generatedStandards = new List<LocationStandardModel>();
        for (int i = 0; i < numberOfStandards; i++)
        {
            generatedStandards.Add(new LocationStandardModel { HasOtherVenues = true });
        }

        queryResult.ProviderLocation.Standards = generatedStandards;

        var source = queryResult.ProviderLocation;

        mediatorMock
            .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(r => r.Ukprn == ukprn && r.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult);

        var result = await sut.DeleteProviderLocationConfirmation(ukprn, id);

        mediatorMock.Verify(m => m.Send(It.IsAny<GetProviderLocationDetailsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult.Model as ProviderLocationConfirmDeleteViewModel;
        model.Should().NotBeNull();
        model.HasNoStandards.Should().Be(hasNoStandards);
        model.HasOneStandard.Should().Be(hasOneStandard);
        model.HasTwoOrMoreStandards.Should().Be(hasMultipleStandards);
    }

    [Test, MoqAutoData]
    public async Task Get_NoMatch_RedirectsToPageNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut)
    {
        var ukprn = 12345678;
        var id = Guid.NewGuid();
        mediatorMock
           .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(r => r.Ukprn == ukprn && r.Id == id), It.IsAny<CancellationToken>()))
           .ReturnsAsync((GetProviderLocationDetailsQueryResult)null);

        var result = await sut.DeleteProviderLocationConfirmation(ukprn, id);

        result.Should().NotBeNull();
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    [Test, MoqAutoData]
    public async Task Get_OrphanedStandard_RedirectsToDeleteDenied(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        GetProviderLocationDetailsQueryResult queryResult,
        Mock<ITempDataDictionary> tempDataMock)
    {
        var ukprn = 12345678;
        var id = Guid.NewGuid();
        foreach (var standard in queryResult.ProviderLocation.Standards)
        {
            standard.HasOtherVenues = false;
            break;
        }

        var expectedValueInTempData = JsonSerializer.Serialize(queryResult);

        mediatorMock
           .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(r => r.Ukprn == ukprn && r.Id == id), It.IsAny<CancellationToken>()))
           .ReturnsAsync(queryResult);

        sut.TempData = tempDataMock.Object;

        var result = await sut.DeleteProviderLocationConfirmation(ukprn, id);

        result.Should().NotBeNull();
        var viewResult = result as RedirectToRouteResult;
        viewResult.Should().NotBeNull();
        viewResult.RouteName.Should().Be("DeleteLocationDenied");
        tempDataMock.Verify(t => t.Add(TempDataKeys.ProviderLocationTempDataKey, expectedValueInTempData));
    }

    [Test, MoqAutoData]
    public async Task Get_UnsuitableUkprn_PageNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        GetProviderLocationDetailsQueryResult queryResult,
        Mock<ITempDataDictionary> tempDataMock)
    {
        var ukprn = 1234567;
        var id = Guid.NewGuid();

        mediatorMock
            .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(r => r.Ukprn == ukprn && r.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult);

        sut.TempData = tempDataMock.Object;

        var result = await sut.DeleteProviderLocationConfirmation(ukprn, id);

        result.Should().NotBeNull();
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    [Test, MoqAutoData]
    public async Task Get_UnsuitableGuid_PageNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        GetProviderLocationDetailsQueryResult queryResult,
        Mock<ITempDataDictionary> tempDataMock)
    {
        var ukprn = 12345678;
        var id = Guid.Empty;

        mediatorMock
            .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(r => r.Ukprn == ukprn && r.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult);

        sut.TempData = tempDataMock.Object;

        var result = await sut.DeleteProviderLocationConfirmation(ukprn, id);

        result.Should().NotBeNull();
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }
}

