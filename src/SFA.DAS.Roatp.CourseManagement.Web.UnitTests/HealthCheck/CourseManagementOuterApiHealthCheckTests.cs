using AutoFixture.NUnit3;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.HealthCheck;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.HealthCheck
{
    public class CourseManagementOuterApiHealthCheckTests
    {
        [Test, MoqAutoData]
        public async Task CheckHealthAsync_ValidQueryResult_ReturnsHealthyStatus(
            [Frozen] Mock<IMediator> mediatorMock,
            HealthCheckContext healthCheckContext,
            GetAllRegionsAndSubRegionsQueryResult queryResult,
            CourseManagementOuterApiHealthCheck healthCheck)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRegionsAndSubRegionsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

            //Act
            var actual = await healthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            Assert.AreEqual(HealthStatus.Healthy, actual.Status);
        }

        [Test, MoqAutoData]
        public async Task CheckHealthAsync_NotValidQueryResult_ReturnsUnHealthyStatus(
            [Frozen] Mock<IMediator> mediatorMock,
            HealthCheckContext healthCheckContext,
            CourseManagementOuterApiHealthCheck healthCheck)
        {
            //Arrange
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRegionsAndSubRegionsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetAllRegionsAndSubRegionsQueryResult());

            //Act
            var actual = await healthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            //Assert
            Assert.AreEqual(HealthStatus.Unhealthy, actual.Status);
        }

        [Test, MoqAutoData]
        public async Task CheckHealthAsync_ExceptionThrown_ReturnsUnHealthyStatus(
            [Frozen] Mock<IMediator> mediatorMock,
            HealthCheckContext healthCheckContext,
            CourseManagementOuterApiHealthCheck healthCheck)
        {
            //Arrange
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRegionsAndSubRegionsQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException());

            //Act
            var actual = await healthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);
            //Assert
            Assert.AreEqual(HealthStatus.Unhealthy, actual.Status);
        }
    }
}