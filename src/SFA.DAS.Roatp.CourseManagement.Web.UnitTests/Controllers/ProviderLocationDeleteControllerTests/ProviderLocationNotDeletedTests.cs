using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AutoFixture.NUnit4;
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
public class ProviderLocationNotDeletedTests
{
    readonly string _getStandardDetailsUrl = Guid.NewGuid().ToString();
    readonly string _getApprenticeshipUnitDetailsUrl = Guid.NewGuid().ToString();

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
            .AddUrlForRoute(RouteNames.GetStandardDetails, _getStandardDetailsUrl)
            .AddUrlForRoute(RouteNames.ManageShortCourseDetails, _getApprenticeshipUnitDetailsUrl);

        foreach (var standard in queryResult.ProviderLocation.Standards)
        {
            standard.HasOtherVenues = false;
        }

        object expectedQueryResult = JsonSerializer.Serialize(queryResult);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ProviderLocationTempDataKey, out expectedQueryResult));

        var result = sut.ProviderLocationNotDeleted(ukprn, id);

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult!.Model as ProviderLocationNotDeletedViewModel;
        model.Should().NotBeNull();
        model.LocationName.Should().Be(queryResult.ProviderLocation.LocationName);
    }

    [Test, MoqAutoData]
    public void WhenStandardsReturnsApprenticeshipAndFoundationApprenticeship_ThenPopulateStandardCourseLinks(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        GetProviderLocationDetailsQueryResult queryResult,
        int ukprn,
        Guid id)
    {
        queryResult.ProviderLocation.Standards = new List<LocationStandardModel>()
            {
                new LocationStandardModel()
                {
                    Title = "Test A Standard",
                    Level = 2,
                    LarsCode = "12345678",
                    LearningType = LearningType.Apprenticeship,
                    HasOtherVenues = false
                },
                new LocationStandardModel()
                {
                    Title = "Test B Foundation Apprenticeship",
                    Level = 2,
                    LarsCode = "23456787",
                    LearningType = LearningType.FoundationApprenticeship,
                    HasOtherVenues = false
                }
            };
        var tempDataMock = new Mock<ITempDataDictionary>();
        sut.TempData = tempDataMock.Object;
        sut.AddDefaultContextWithUser().AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.GetStandardDetails, _getStandardDetailsUrl)
            .AddUrlForRoute(RouteNames.ManageShortCourseDetails, _getApprenticeshipUnitDetailsUrl);

        object expectedQueryResult = JsonSerializer.Serialize(queryResult);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ProviderLocationTempDataKey, out expectedQueryResult));

        var result = sut.ProviderLocationNotDeleted(ukprn, id);

        var viewResult = result as ViewResult;
        var model = viewResult!.Model as ProviderLocationNotDeletedViewModel;
        var standardCourses = model.StandardLinks.Courses.ToList();
        standardCourses[0].CourseName.Should().Be("Test A Standard (level 2)");
        standardCourses[0].Url.Should().Be(_getStandardDetailsUrl);
        standardCourses[1].CourseName.Should().Be("Test B Foundation Apprenticeship (level 2)");
        standardCourses[1].Url.Should().Be(_getStandardDetailsUrl);
        model.StandardLinks.Courses.Count().Should().Be(2);
    }

    [Test, MoqAutoData]
    public void WhenStandardsReturnsApprenticeshipUnit_ThenPopulateApprenticeshipUnitCourseLinks(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        GetProviderLocationDetailsQueryResult queryResult,
        int ukprn,
        Guid id)
    {
        queryResult.ProviderLocation.Standards = new List<LocationStandardModel>()
            {
                new LocationStandardModel()
                {
                    Title = "Test Apprenticeship Unit",
                    Level = 2,
                    LarsCode = "98765432",
                    LearningType = LearningType.ApprenticeshipUnit,
                    HasOtherVenues = false
                }
            };
        var tempDataMock = new Mock<ITempDataDictionary>();
        sut.TempData = tempDataMock.Object;
        sut.AddDefaultContextWithUser().AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.GetStandardDetails, _getStandardDetailsUrl)
            .AddUrlForRoute(RouteNames.ManageShortCourseDetails, _getApprenticeshipUnitDetailsUrl);

        object expectedQueryResult = JsonSerializer.Serialize(queryResult);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ProviderLocationTempDataKey, out expectedQueryResult));

        var result = sut.ProviderLocationNotDeleted(ukprn, id);

        var viewResult = result as ViewResult;
        var model = viewResult!.Model as ProviderLocationNotDeletedViewModel;
        model.ApprenticeshipUnitLinks.Courses.First().CourseName.Should().Be("Test Apprenticeship Unit (level 2)");
        model.ApprenticeshipUnitLinks.Courses.First().Url.Should().Be(_getApprenticeshipUnitDetailsUrl);
    }

    [Test]
    [MoqInlineAutoData(true, true, true, false, false)]
    [MoqInlineAutoData(true, false, true, true, false)]
    [MoqInlineAutoData(false, true, true, true, false)]
    [MoqInlineAutoData(false, false, true, true, false)]
    [MoqInlineAutoData(true, true, false, false, true)]
    [MoqInlineAutoData(false, false, false, true, true)]
    public void WhenStandardsAreReturnedAndHasOtherVenuesIsTrueOrFalse_ThenShowCourseFlagsAreSetCorrectly(
        bool hasOtherVenuesStandard,
        bool hasOtherVenuesFoundationApprenticeship,
        bool hasOtherVenuesApprenticeshipUnit,
        bool expectedShowStandards,
        bool expectedShowApprenticeshipUnits,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        GetProviderLocationDetailsQueryResult queryResult,
        int ukprn,
        Guid id)
    {
        queryResult.ProviderLocation.Standards = new List<LocationStandardModel>()
            {
                new LocationStandardModel()
                {
                    Title = "Test Standard",
                    Level = 2,
                    LarsCode = "12345678",
                    LearningType = LearningType.Apprenticeship,
                    HasOtherVenues = hasOtherVenuesStandard
                },
                new LocationStandardModel()
                {
                    Title = "Test Foundation Apprenticeship",
                    Level = 2,
                    LarsCode = "23456787",
                    LearningType = LearningType.FoundationApprenticeship,
                    HasOtherVenues = hasOtherVenuesFoundationApprenticeship
                },
                new LocationStandardModel()
                {
                    Title = "Test Apprenticeship Unit",
                    Level = 2,
                    LarsCode = "98765432",
                    LearningType = LearningType.ApprenticeshipUnit,
                    HasOtherVenues = hasOtherVenuesApprenticeshipUnit
                }
            };
        var tempDataMock = new Mock<ITempDataDictionary>();
        sut.TempData = tempDataMock.Object;
        sut.AddDefaultContextWithUser().AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.GetStandardDetails, _getStandardDetailsUrl)
            .AddUrlForRoute(RouteNames.ManageShortCourseDetails, _getApprenticeshipUnitDetailsUrl);

        object expectedQueryResult = JsonSerializer.Serialize(queryResult);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ProviderLocationTempDataKey, out expectedQueryResult));

        var result = sut.ProviderLocationNotDeleted(ukprn, id);

        var viewResult = result as ViewResult;
        var model = viewResult!.Model as ProviderLocationNotDeletedViewModel;
        model.ShowStandards.Should().Be(expectedShowStandards);
        model.ShowApprenticeshipUnits.Should().Be(expectedShowApprenticeshipUnits);
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
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ProviderLocationTempDataKey, out expectedQueryResult));

        var result = sut.ProviderLocationNotDeleted(ukprn, id);

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }
}