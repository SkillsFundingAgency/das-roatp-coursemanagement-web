using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    public class GetStandardQueryHandlerTests
    {
        private GetStandardQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private Mock<ILogger<GetStandardQueryHandler>> _logger;
        private GetStandardQuery _query;
        private GetStandardQueryResult _queryResult;
        private List<Domain.ApiModels.Standard> standards;

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetStandardQuery>();
            _queryResult = autoFixture.Create<GetStandardQueryResult>();
            standards = autoFixture.Create<List<Domain.ApiModels.Standard>>();
            _apiClient = new Mock<IApiClient>();
            _logger = new Mock<ILogger<GetStandardQueryHandler>>();

        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsValidResponse()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.Standard>>($"/Standards/{_query.Ukprn}")).ReturnsAsync(() => standards);
            _handler = new GetStandardQueryHandler(_apiClient.Object, _logger.Object);

            var result = await _handler.Handle(_query, CancellationToken.None);

            var compareLogic = new CompareLogic(new ComparisonConfig { IgnoreObjectTypes = true });
            var comparisonResult = compareLogic.Compare(_queryResult.Standards, result);
            Assert.IsTrue(comparisonResult.AreEqual);
        }

        [Test]
        public async Task Handle_NoStandardsReturned_ReturnsNullResponse()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.Standard>>($"/Standards/{_query.Ukprn}")).ReturnsAsync(() => null);
            _handler = new GetStandardQueryHandler(_apiClient.Object, _logger.Object);

            var result = await _handler.Handle(_query, CancellationToken.None);

            Assert.IsNull(result);
        }

        [Test]
        public void Handle_NoStandardsRetruned_ReturnsNullResponse()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.Standard>>($"/Standards/{_query.Ukprn}")).Throws(new Exception());
            _handler = new GetStandardQueryHandler(_apiClient.Object, _logger.Object);
            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_query, CancellationToken.None));
        }
    }
}
