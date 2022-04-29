using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.ApiClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.ApiClients
{
    [TestFixture]
    public class RoatpCourseManagementOuterApiClientTests
    {
        private const string RoatpCourseManagementOuterApiBaseAddress = "http://localhost:5334";

        //private IConfigurationService _config;
        private RoatpCourseManagementOuterApiClient _apiClient;

        [SetUp]
        public void Before_each_test()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("[{'ProviderCourseId':1,'CourseName':'Test','Level':1,'IsImported':'false'}]", Encoding.UTF8, "application/json"),
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(RoatpCourseManagementOuterApiBaseAddress),
            };
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");


            var logger = new Mock<ILogger<RoatpCourseManagementOuterApiClient>>();

            _apiClient = new RoatpCourseManagementOuterApiClient(httpClient, logger.Object);
        }

        [Test]
        public async Task RoatpCourseManagementOuterApiClient_Retrieves_listOfStandards()
        {
            int ukprn = 111;
            var result = await _apiClient.GetAllStandards(ukprn);

            result.Should().NotBeNull();
        }
    }
}
