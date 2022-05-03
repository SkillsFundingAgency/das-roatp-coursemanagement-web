using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    public class GetStandardQueryHandlerTests
    {
        private GetStandardQueryHandler _handler;
        private Mock<IGetStandardsApiClient> _apiClient;
        private Mock<ILogger<GetStandardQueryHandler>> _logger;
        private GetStandardQuery _query;
        private GetStandardQueryResult _queryResult;
        private List<Domain.Standards.Standard> standards;

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetStandardQuery>();
            _queryResult = autoFixture.Create<GetStandardQueryResult>();
            standards = autoFixture.Create<List<Domain.Standards.Standard>>();

            _apiClient = new Mock<IGetStandardsApiClient>();
            _apiClient.Setup(x => x.GetAllStandards(_query.Ukprn)).ReturnsAsync(() => standards);
            _logger = new Mock<ILogger<GetStandardQueryHandler>>();
            _handler = new GetStandardQueryHandler(_apiClient.Object, _logger.Object);
        }

        [Test]
        public async Task GetStandardQueryHandler_Returns_Response()
        {
            var result = await _handler.Handle(_query, CancellationToken.None);

            var compareLogic = new CompareLogic(new ComparisonConfig { IgnoreObjectTypes = true });
            var comparisonResult = compareLogic.Compare(_queryResult.Standards, result);
            Assert.IsTrue(comparisonResult.AreEqual);
        }
    }
}
