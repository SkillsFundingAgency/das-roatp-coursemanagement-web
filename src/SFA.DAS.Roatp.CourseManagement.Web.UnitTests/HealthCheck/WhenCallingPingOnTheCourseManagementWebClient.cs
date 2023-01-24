using AutoFixture.NUnit3;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Roatp.CourseManagement.Web.HealthCheck;
using SFA.DAS.Testing.AutoFixture;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.HealthCheck
{
    public class WhenCallingPingOnTheCourseManagementWebClient
    {
        [Test, MoqAutoData]
        public async Task Then_The_Ping_Endpoint_Is_Called_For_CourseManagementWeb(
            [Frozen] Mock<IApiClient> client,
            HealthCheckContext healthCheckContext,
            CourseManagementOuterApiHealthCheck healthCheck)
        {
            //Act
            await healthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);
            //Assert
            client.Verify(x => x.Get("ping"), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_If_It_Is_Successful_200_Is_Returned(
            [Frozen] Mock<IApiClient> client,
            HealthCheckContext healthCheckContext,
            CourseManagementOuterApiHealthCheck healthCheck)
        {
            //Arrange
            client.Setup(x => x.Get("ping"))
                .ReturnsAsync(HttpStatusCode.OK);
            //Act
            var actual = await healthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);
            //Assert
            Assert.AreEqual(HealthStatus.Healthy, actual.Status);
        }

        [Test, MoqAutoData]
        public async Task And_CourseDeliveryApi_Ping_Not_Found_Then_Unhealthy(
            [Frozen] Mock<IApiClient> client,
            HealthCheckContext healthCheckContext,
            CourseManagementOuterApiHealthCheck healthCheck)
        {
            //Arrange
            client.Setup(x => x.Get("ping"))
                .ReturnsAsync(HttpStatusCode.NotFound);
            //Act
            var actual = await healthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);
            //Assert
            Assert.AreEqual(HealthStatus.Unhealthy, actual.Status);
        }
    }
}