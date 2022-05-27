using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using System;
using System.Security.Claims;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ErrorControllerTests
{
    [TestFixture]
    public class ErrorControllerTests
    {
        const string PageNotFoundViewName = "PageNotFound";
        const string ErrorInServiceViewName = "ErrorInService";

        [TestCase(403, PageNotFoundViewName)]
        [TestCase(404, PageNotFoundViewName)]
        [TestCase(500, ErrorInServiceViewName)]
        public void HttpStatusCodeHandler_ReturnsRespectiveView(int statusCode, string expectedViewName)
        {
            var sut = new ErrorController(Mock.Of<ILogger<ErrorController>>());

            ViewResult result = (ViewResult)sut.HttpStatusCodeHandler(statusCode);

            result.ViewName.Should().Be(expectedViewName);
        }

        
    }
}
