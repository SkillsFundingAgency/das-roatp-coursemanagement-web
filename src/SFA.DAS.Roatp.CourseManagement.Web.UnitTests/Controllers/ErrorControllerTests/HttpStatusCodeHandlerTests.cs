using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ErrorControllerTests;

[TestFixture]
public class ErrorControllerTests
{
    [TestCase(403, ErrorController.ProviderNotRegisteredViewName)]
    [TestCase(404, ErrorController.PageNotFoundViewName)]
    [TestCase(500, ErrorController.ErrorInServiceViewName)]
    public void HttpStatusCodeHandler_ReturnsRespectiveView(int statusCode, string expectedViewName)
    {
        var sut = new ErrorController(Mock.Of<ILogger<ErrorController>>());

        ViewResult result = (ViewResult)sut.HttpStatusCodeHandler(statusCode);

        result.ViewName.Should().Be(expectedViewName);
    }
}
