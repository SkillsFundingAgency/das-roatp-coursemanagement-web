using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers;

[TestFixture]
public class ReviewYourDetailsControllerGetTests
{
    private const string SelectCourseTypeUrl = "http://test/view-standards";
    private const string ProviderLocationsUrl = "http://test/provider-locations";
    private const string ProviderDescriptionUrl = "http://test/provider-description";
    private const string ProviderContactUrl = "http://test/provider-contact";
    private const string ForecastCoursesUrl = "http://test/forecasts/courses";

    Mock<ISessionService> _sessionServiceMock;
    Mock<IMediator> _mediatorMock;
    ReviewYourDetailsController _sut;

    [SetUp]
    public void Before_Each_Test()
    {
        _sessionServiceMock = new();
        _mediatorMock = new();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAllProviderStandardsQueryResult() { Standards = [new Standard()] });
        _sut = new(_sessionServiceMock.Object, _mediatorMock.Object);

        _sut
            .AddDefaultContextWithUser()
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.SelectCourseType, SelectCourseTypeUrl)
            .AddUrlForRoute(RouteNames.GetProviderLocations, ProviderLocationsUrl)
            .AddUrlForRoute(RouteNames.GetProviderDescription, ProviderDescriptionUrl)
            .AddUrlForRoute(RouteNames.CheckProviderContactDetails, ProviderContactUrl)
            .AddUrlForRoute(RouteNames.ForecastCourses, ForecastCoursesUrl);
    }

    [Test]
    public async Task OnGet_ClearsSession()
    {
        await _sut.ReviewYourDetails(default);

        _sessionServiceMock.Verify(s => s.Delete(nameof(ProviderContactSessionModel)));
    }

    [Test]
    public async Task OnGet_HasNoShortCourse_SetsShowForecastOptionToTrue()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAllProviderStandardsQueryResult() { Standards = [] });

        var result = await _sut.ReviewYourDetails(default);
        result.As<ViewResult>().Model.As<ReviewYourDetailsViewModel>().ShowForecastOption.Should().BeFalse();
    }

    [Test]
    public async Task Index_ReturnsViewWithExpectedUrlsInModel()
    {
        var expectedModel = new ReviewYourDetailsViewModel()
        {
            ProviderLocationsUrl = ProviderLocationsUrl,
            SelectCourseTypeUrl = SelectCourseTypeUrl,
            ProviderDescriptionUrl = ProviderDescriptionUrl,
            ProviderContactUrl = ProviderContactUrl,
            ForecastUrl = ForecastCoursesUrl,
            ShowForecastOption = true
        };

        var result = await _sut.ReviewYourDetails(default);
        result.As<ViewResult>().Model.As<ReviewYourDetailsViewModel>().Should().BeEquivalentTo(expectedModel);
    }
}
