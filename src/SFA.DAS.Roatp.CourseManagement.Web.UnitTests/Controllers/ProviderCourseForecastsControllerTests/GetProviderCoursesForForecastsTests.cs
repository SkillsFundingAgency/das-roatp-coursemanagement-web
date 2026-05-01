using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Forecasts;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseForecastsControllerTests;

public class GetProviderCoursesForForecastsTests
{
    private readonly Standard _expectedStandard = new() { CourseName = "course name", Level = 1 };
    private Mock<IValidator<CourseForecastsSubmitModel>> _validatorMock;
    ProviderCourseForecastsController _sut;
    ForecastCoursesViewModel _actualModel;

    [SetUp]
    public async Task BeforeEachTest()
    {
        Mock<IMediator> mediatorMock = new();
        GetAllProviderStandardsQueryResult coursesResult = new() { Standards = [_expectedStandard] };
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(coursesResult);
        _validatorMock = new Mock<IValidator<CourseForecastsSubmitModel>>();
        _sut = new(mediatorMock.Object, _validatorMock.Object);

        _sut
            .AddDefaultContextWithUser()
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CourseForecasts);

        var result = await _sut.GetProviderCoursesForForecasts(default);
        _actualModel = result.As<ViewResult>().Model.As<ForecastCoursesViewModel>();
    }

    [Test]
    public async Task GetProviderCoursesForForecasts_SetsCorrectApprenticeshipTypeInModel()
    {
        _actualModel.ApprenticeshipType.Should().Be(ApprenticeshipType.ApprenticeshipUnit);
    }

    [Test]
    public async Task GetProviderCoursesForForecasts_PopulatedCourseLinks()
    {
        _actualModel.CourseLinks.Courses.Should().HaveCount(1);
        CourseLink link = _actualModel.CourseLinks.Courses.First();
        link.Name.Should().Be(_expectedStandard.DisplayName);
        link.Url.Should().Be(TestConstants.DefaultUrl);
    }
}
