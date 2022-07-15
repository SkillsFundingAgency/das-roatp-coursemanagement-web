using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Text;
using System.Text.Json;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Services.SessionServiceTests
{
    [TestFixture]
    public class SessionServiceSetTests
    {
        private Mock<ISession> _sessionMock;
        private SessionService _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _sessionMock = new Mock<ISession>();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _sessionMock.Object;
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);
            _sut = new SessionService(httpContextAccessorMock.Object);
        }

        [Test, AutoData]
        public void Set_AddsValueToSession(string key, string context, string value)
        {
            _sut.Set(value, key, context);

            _sessionMock.Verify(s => s.Set(key, Encoding.UTF8.GetBytes(value)));
            _sessionMock.Verify(s => s.Set(SessionService.ContextKey, Encoding.UTF8.GetBytes(context)));
        }

        [Test, AutoData]
        public void SetOfT_AddsSerialisedValueToSession(string context, Person value)
        {
            var json = JsonSerializer.Serialize(value);

            _sut.Set(value, context);

            _sessionMock.Verify(s => s.Set(nameof(Person), Encoding.UTF8.GetBytes(json)));
            _sessionMock.Verify(s => s.Set(SessionService.ContextKey, Encoding.UTF8.GetBytes(context)));
        }

        [Test, AutoData]
        public void Get_MatchingContext_GetsValueForGivenKey(string key, string value, string context)
        {
            var contextValue = Encoding.UTF8.GetBytes(context);
            _sessionMock.Setup(s => s.TryGetValue(SessionService.ContextKey, out contextValue)).Returns(true);
            var keyValue = Encoding.UTF8.GetBytes(value);
            _sessionMock.Setup(s => s.TryGetValue(key, out keyValue)).Returns(true);

            var actual = _sut.Get(key, context);

            actual.Should().Be(value);
        }

        [Test, AutoData]
        public void Get_ContextNotMatching_ReturnsNull(string key, string value, string context)
        {
            byte[] contextValue;
            _sessionMock.Setup(s => s.TryGetValue(SessionService.ContextKey, out contextValue)).Returns(false);

            var actual = _sut.Get(key, context);

            _sessionMock.Verify(s => s.TryGetValue(key, out It.Ref<byte[]>.IsAny), Times.Never);

            actual.Should().BeNull();
        }

        [Test, AutoData]
        public void GetOfT_MatchingContext_GetsValueForGivenKey(string key, Person value, string context)
        {
            var json = JsonSerializer.Serialize(value);
            var contextValue = Encoding.UTF8.GetBytes(context);
            _sessionMock.Setup(s => s.TryGetValue(SessionService.ContextKey, out contextValue)).Returns(true);
            var keyValue = Encoding.UTF8.GetBytes(json);
            _sessionMock.Setup(s => s.TryGetValue(key, out keyValue)).Returns(true);

            var actual = _sut.Get(key, context);

            actual.Should().Be(json);
        }

        [Test, AutoData]
        public void Delete_MatchingContext_RemovesKey(string key, string context)
        {
            var contextValue = Encoding.UTF8.GetBytes(context);
            _sessionMock.Setup(s => s.TryGetValue(SessionService.ContextKey, out contextValue)).Returns(true);
            _sessionMock.Setup(s => s.Keys).Returns(new [] { key });

            _sut.Delete(key, context);

            _sessionMock.Verify(s => s.Remove(key));
        }

        [Test, AutoData]
        public void Delete_ContextNotMatching_RemovesKey(string key, string context)
        {
            byte[] contextValue;
            _sessionMock.Setup(s => s.TryGetValue(SessionService.ContextKey, out contextValue)).Returns(false);

            _sut.Delete(key, context);

            _sessionMock.Verify(s => s.Remove(It.IsAny<string>()), Times.Never);
        }

        [Test, AutoData]
        public void DeleteOfT_MatchingContext_RemovesKey(Person value, string context)
        {
            var key = typeof(Person).Name;
            var contextValue = Encoding.UTF8.GetBytes(context);
            _sessionMock.Setup(s => s.TryGetValue(SessionService.ContextKey, out contextValue)).Returns(true);
            _sessionMock.Setup(s => s.Keys).Returns(new[] { key });

            _sut.Delete(key, context);

            _sessionMock.Verify(s => s.Remove(key));
        }

        [Test]
        public void Clear_CallsSesssionClear()
        {
            _sut.Clear();
            _sessionMock.Verify(s => s.Clear());
        }

        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
