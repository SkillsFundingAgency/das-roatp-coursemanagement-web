using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditNationalDeliveryOptionControllerTests;

[TestFixture]
public class EditNationalDeliveryOptionGetTests : EditNationalDeliveryOptionControllerTestBase
{
    [Test]
    public void WhenSessionIsNotSet_ThenReturnsNotFoundResponse()
    {
        SetupController();

        var result = Sut.Index(LarsCode);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public void WhenSelectedOptionDoesNotHaveEmployer_ThenRedirectToLocationOptionQuestion()
    {
        SetupController();
        SetLocationOptionInSession(LocationOption.ProviderLocation);

        var result = Sut.Index(LarsCode);

        result.Should().BeOfType<RedirectToRouteResult>().Which.RouteName.Should().Be(RouteNames.GetLocationOption);
    }

    [TestCase(LocationOption.Both)]
    [TestCase(LocationOption.EmployerLocation)]
    public void WhenSelectedOptionHasEmployer_ThenRedirectToLocationOptionQuestion(LocationOption option)
    {
        SetupController();

        SetLocationOptionInSession(option);

        var result = Sut.Index(LarsCode);

        result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<EditNationalDeliveryOptionViewModel>();
    }
}
