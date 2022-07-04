using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllStandardRegions;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    [TestFixture]
    public class GetAllStandardRegionsQueryHandlerTests
    {
        private GetAllStandardRegionsQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private GetAllStandardRegionsQuery _query;
        private GetAllStandardRegionsQueryResult _result;

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetAllStandardRegionsQuery>();
            _result = autoFixture.Create<GetAllStandardRegionsQueryResult>();
            _apiClient = new Mock<IApiClient>();
            _handler = new GetAllStandardRegionsQueryHandler(_apiClient.Object, Mock.Of<ILogger<GetAllStandardRegionsQueryHandler>>());
        }
        
        [Test]
        public async Task Handle_ValidApiRequest_ReturnsValidResponseNoSelectRegions()
        {
            _apiClient.Setup(x => x.Get<GetAllStandardRegionsQueryResult>($"providers/{_query.Ukprn}/courses/{_query.LarsCode}/standardsubregions")).ReturnsAsync(_result);

            var result = await _handler.Handle(_query, CancellationToken.None);

            result.Should().NotBeNull();
        }

        [Test]
        public void Handle_NoRegionsData_ThrowsException()
        {
            _apiClient.Setup(x => x.Get<GetAllStandardRegionsQueryResult>($"providers/{_query.Ukprn}/courses/{_query.LarsCode}/standardsubregions")).ReturnsAsync((GetAllStandardRegionsQueryResult)null);

            Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(_query, CancellationToken.None));
        }
    }
}
