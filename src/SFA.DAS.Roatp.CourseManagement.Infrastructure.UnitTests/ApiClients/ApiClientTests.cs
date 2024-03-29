﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Infrastructure.ApiClients;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Infrastructure.UnitTests.ApiClients
{
    [TestFixture]
    public class ApiClientTests
    {
        private const string RoatpCourseManagementOuterApiBaseAddress = "http://localhost:5334";
        private ApiClient _apiClient;

        [SetUp]
        public void Before_each_test()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("[{'ProviderCourseId':1,'CourseName':'Test','Level':1,'IsImported':'false'}]", Encoding.UTF8, "application/json"),
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(RoatpCourseManagementOuterApiBaseAddress),
            };
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");


            var logger = new Mock<ILogger<ApiClient>>();

            _apiClient = new ApiClient(httpClient, logger.Object);
        }

        [Test]
        public async Task ApiClient_GetMethodRetrieves_listOfStandards()
        {
            int ukprn = 111;
            var result = await _apiClient.Get<List<Domain.ApiModels.Standard>>($"/Standards/{ukprn}");

            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(0);
        }
        [Test]
        public async Task ApiClient_GetMethodReturns_NullResponse()
        {
            int ukprn = 111;
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.InternalServerError))
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(RoatpCourseManagementOuterApiBaseAddress),
            };
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var logger = new Mock<ILogger<ApiClient>>();

            _apiClient = new ApiClient(httpClient, logger.Object);

            var result = await _apiClient.Get<List<Domain.ApiModels.Standard>>($"/Standards/{ukprn}");

            result.Should().BeNull();
        }

        [Test]
        public void ApiClient_GetMethodReturns_Exception()
        {
            int ukprn = 111;
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(() => null)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(RoatpCourseManagementOuterApiBaseAddress),
            };
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var logger = new Mock<ILogger<ApiClient>>();

            _apiClient = new ApiClient(httpClient, logger.Object);

            Assert.ThrowsAsync<InvalidOperationException>(async() => await _apiClient.Get<List<Domain.ApiModels.Standard>>($"/Standards/{ukprn}"));
        }

        [Test]
        public async Task ApiClient_PostMethodReturns_Exception()
        {
            var result = await _apiClient.Post($"/Test", new List<Domain.ApiModels.Standard>());

            result.Should().Be(HttpStatusCode.OK);
        }

    }
}
