﻿using AutoFixture.NUnit3;
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
        public void Set_AddsValueToSession(string key, string value)
        {
            _sut.Set(value, key);

            _sessionMock.Verify(s => s.Set(key, Encoding.UTF8.GetBytes(value)));
        }

        [Test, AutoData]
        public void SetOfT_AddsSerialisedValueToSession(string context, Person value)
        {
            var json = JsonSerializer.Serialize(value);

            _sut.Set(value);

            _sessionMock.Verify(s => s.Set(nameof(Person), Encoding.UTF8.GetBytes(json)));
        }

        [Test, AutoData]
        public void Get_GetsValueForGivenKey(string key, string value)
        {
            var keyValue = Encoding.UTF8.GetBytes(value);
            _sessionMock.Setup(s => s.TryGetValue(key, out keyValue)).Returns(true);

            var actual = _sut.Get(key);

            actual.Should().Be(value);
        }

        [Test, AutoData]
        public void Get_ContextNotMatching_ReturnsNull(string key)
        {
            byte[] contextValue;
            _sessionMock.Setup(s => s.TryGetValue(key, out contextValue)).Returns(false);

            var actual = _sut.Get(key);

            _sessionMock.Verify(s => s.TryGetValue(key, out It.Ref<byte[]>.IsAny), Times.Once);

            actual.Should().BeNull();
        }

        [Test, AutoData]
        public void GetOfT_GetsValueForGivenKey(string key, Person value)
        {
            var json = JsonSerializer.Serialize(value);
    
            var keyValue = Encoding.UTF8.GetBytes(json);
            _sessionMock.Setup(s => s.TryGetValue(key, out keyValue)).Returns(true);

            var actual = _sut.Get(key);

            actual.Should().Be(json);
        }

        [Test, AutoData]
        public void GetOfT_GetsValueForGivenType(Person value)
        {
            var json = JsonSerializer.Serialize(value);
            var key = typeof(Person).Name;

            var keyValue = Encoding.UTF8.GetBytes(json);
            _sessionMock.Setup(s => s.TryGetValue(key, out keyValue)).Returns(true);

            var actual = _sut.Get<Person>();

            actual.Should().BeEquivalentTo(value);
        }

        [Test, AutoData]
        public void Delete_RemovesKey(string key)
        {
            _sessionMock.Setup(s => s.Keys).Returns(new [] { key });

            _sut.Delete(key);

            _sessionMock.Verify(s => s.Remove(key));
        }

        [Test, AutoData]
        public void Delete_KeyDoesNotExist_ReturnsWithoutError(string key)
        {
            _sut.Delete(key);

            _sessionMock.Verify(s => s.Remove(It.IsAny<string>()), Times.Never);
        }

        [Test, AutoData]
        public void DeleteOfT_ObjectFound_RemovesObjectFromSession()
        {
            var key = typeof(Person).Name;
            _sessionMock.Setup(s => s.Keys).Returns(new[] { key });

            _sut.Delete(key);

            _sessionMock.Verify(s => s.Remove(key));
        }

        [Test, AutoData]
        public void DeleteOfT_RemovesKeyByType()
        {
            var key = typeof(Person).Name;
            _sessionMock.Setup(s => s.Keys).Returns(new[] { key });

            _sut.Delete<Person>(new Person());

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
