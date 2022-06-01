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
    public class ErrorInServiceTests
    {
        private const string userId = "isp/10012002s";
        private Mock<HttpContext> httpContextMock = new Mock<HttpContext>();
        private InMemoryFakeLogger<ErrorController> loggerFake = new InMemoryFakeLogger<ErrorController>();
        private Exception exception = new Exception("Something went wrong");
        private const string path = "/providers/10012002";
        private ErrorController sut;

        [SetUp]
        public void Before_Each_Test()
        {
            var featuresMock = new Mock<IFeatureCollection>();

            featuresMock.Setup(f => f.Get<IExceptionHandlerPathFeature>())
                .Returns(new ExceptionHandlerFeature
                {
                    Path = path,
                    Error = exception
                });
            httpContextMock.Setup(p => p.Features).Returns(featuresMock.Object);

            sut = new ErrorController(loggerFake)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object,
                }
            };
        }

        [Test]
        public void ErrorInService_UserIsAuthenticatedLogErrorAndReturnsErrorInServiceView()
        {
            var authorisedUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ProviderClaims.ProviderUkprn, "10012002"),
                new Claim(ProviderClaims.UserId, userId)
            }, "mock"));
            httpContextMock.Setup(c => c.User).Returns(authorisedUser);

            var result = (ViewResult)sut.ErrorInService();

            result.Should().NotBeNull();
            loggerFake.Message.Contains(userId).Should().BeTrue();
            loggerFake.Message.Contains(path).Should().BeTrue();
        }

        [Test]
        public void ErrorInService_UserIsNotAuthenticated_LogErrorAndReturnsErrorInServiceView()
        {
            var unauthorisedUser = new ClaimsPrincipal(new ClaimsIdentity());
            httpContextMock.Setup(c => c.User).Returns(unauthorisedUser);

            var sut = new ErrorController(loggerFake)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object,
                }
            };

            var result = (ViewResult)sut.ErrorInService();

            result.Should().NotBeNull();
            loggerFake.Message.Contains(userId).Should().BeFalse();
            loggerFake.Message.Contains(path).Should().BeTrue();
        }

        public class InMemoryFakeLogger<T> : ILogger<T>
        {
            public LogLevel Level { get; private set; }
            public Exception Ex { get; private set; }
            public string Message { get; private set; }

            public IDisposable BeginScope<TState>(TState state)
            {
                return NullScope.Instance;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Level = logLevel;
                Message = state.ToString();
                Ex = exception;
            }

            public class NullScope : IDisposable
            {
                public static NullScope Instance { get; } = new NullScope();

                private NullScope()
                {
                }

                public void Dispose()
                {
                }
            }
        }
    }
}
