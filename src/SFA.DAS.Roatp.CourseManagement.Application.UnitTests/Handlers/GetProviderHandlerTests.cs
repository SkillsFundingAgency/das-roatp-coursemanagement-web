using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    [TestFixture]
    public class GetProviderHandlerTests
    {
        private GetProviderQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private GetProviderQuery _query;
        private readonly Provider _provider = new Provider();
        private const int Ukprn = 12345678;
        private const string MarketingInfo = "Marketing info";
        [SetUp]
        public void Setup()
        {
            _query = new GetProviderQuery(Ukprn);
            _apiClient = new Mock<IApiClient>();
            _handler = new GetProviderQueryHandler(_apiClient.Object, Mock.Of<ILogger<GetProviderQueryHandler>>());
        }

        [Test]
        public async Task Handle_ValidApiRequest_ReturnsValidResponse()
        {
            _provider.MarketingInfo = MarketingInfo;
            _apiClient.Setup(x => x.Get<Provider>($"providers/{_query.Ukprn}")).ReturnsAsync(_provider);
            var result = await _handler.Handle(_query, CancellationToken.None);
            result.Should().NotBeNull();
            result.Provider.Should().BeEquivalentTo(_provider);
        }

        [Test]
        public async Task Handle_InvalidApiResponse_ReturnsNull()
        {
            _apiClient.Setup(x => x.Get<Provider>($"providers/{_query.Ukprn}")).ReturnsAsync((Provider)null);

            var result = await _handler.Handle(_query, CancellationToken.None);
            result.Provider.Should().BeNull();
        }
    }
}
