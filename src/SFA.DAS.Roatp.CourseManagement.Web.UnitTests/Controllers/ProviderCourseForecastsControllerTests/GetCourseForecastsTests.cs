using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Forecasts;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseForecastsControllerTests;

public class GetCourseForecastsTests
{
    private IActionResult _actual;
    private Mock<IMediator> _mediatorMock;
    private Mock<IValidator<CourseForecastsSubmitModel>> _validatorMock;
    private GetProviderCourseForecastsQuery _query;
    private GetProviderCourseForecastsQueryResult _queryResult;

    [SetUp]
    public async Task Arrange()
    {
        Fixture fixture = new();
        _queryResult = fixture.Create<GetProviderCourseForecastsQueryResult>();
        _query = fixture.Create<GetProviderCourseForecastsQuery>();

        _mediatorMock = new();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderCourseForecastsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(_queryResult);
        _validatorMock = new Mock<IValidator<CourseForecastsSubmitModel>>();
        ProviderCourseForecastsController sut = new(_mediatorMock.Object, _validatorMock.Object);
        sut.AddDefaultContextWithUser();

        _actual = await sut.GetCourseForecasts(_query.LarsCode, default);

    }

    [Test]
    public void GetCourseForecasts_InvokesMediatorWithCorrectQuery()
    {
        _mediatorMock.Verify(m => m.Send(It.Is<GetProviderCourseForecastsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == _query.LarsCode), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void GetCourseForecasts_ReturnsRespectiveView()
    {
        _actual.As<ViewResult>().ViewName.Should().Be(ProviderCourseForecastsController.CourseForecastsViewPath);
    }

    [Test]
    public void GetCourseForecasts_ReturnsExpectedViewModel()
    {
        CourseForecastsViewModel actualModel = _actual.As<ViewResult>().Model.As<CourseForecastsViewModel>();

        actualModel.ApprenticeshipType.Should().Be(ApprenticeshipType.ApprenticeshipUnit);
        actualModel.CourseDisplayName.Should().Be($"{_queryResult.CourseName} (level {_queryResult.CourseLevel})");
        actualModel.Ukprn.ToString().Should().Be(TestConstants.DefaultUkprn);
        actualModel.LarsCode.Should().Be(_query.LarsCode);
        actualModel.LastUpdatedDate.Should().Be(_queryResult.Forecasts.Max(c => c.UpdatedDate)?.ToString("dd MMMM yyyy"));
        actualModel.Forecasts.Should().HaveCount(_queryResult.Forecasts.Count());
    }
}

public class PostCourseForecastsTests
{
    [Test, MoqAutoData]
    public async Task PostCourseForecasts_InvalidState_ReturnsView(
        GetProviderCourseForecastsQuery query,
        GetProviderCourseForecastsQueryResult queryResult,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderCourseForecastsController sut)
    {
        mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderCourseForecastsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        sut.AddDefaultContextWithUser();
        sut.ModelState.AddModelError("key", "value");

        var response = await sut.PostCourseForecasts("zsc00001", new CourseForecastsSubmitModel(), default);

        response.As<ViewResult>().ViewName.Should().Be(ProviderCourseForecastsController.CourseForecastsViewPath);
    }

    [Test, MoqAutoData]
    public async Task PostCourseForecasts_ValidState_InvokesMediator(
        CourseForecastsSubmitModel submitModel,
        string larsCode,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<CourseForecastsSubmitModel>> validator,
        [Greedy] ProviderCourseForecastsController sut)
    {
        validator.Setup(x => x.Validate(It.IsAny<CourseForecastsSubmitModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        await sut.PostCourseForecasts(larsCode, submitModel, default);

        mediatorMock.Verify(m => m.Send(It.Is<UpsertProviderCourseForecastsCommand>(c => c.Ukprn.ToString() == TestConstants.DefaultUkprn && c.LarsCode == larsCode && c.Forecasts.Count() == submitModel.Forecasts.Count), It.IsAny<CancellationToken>()), Times.Once);
    }
}
