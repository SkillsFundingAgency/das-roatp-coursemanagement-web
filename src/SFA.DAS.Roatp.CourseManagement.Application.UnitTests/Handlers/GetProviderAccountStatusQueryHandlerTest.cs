using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries.GetProviderAccount;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    [TestFixture]
    public class GetProviderAccountStatusQueryHandlerTest
    {
        private GetProviderAccountStatusQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private GetProviderAccountStatusQuery _query;
        private readonly ProviderAccountResponse _provider = new();
        private const int Ukprn = 12345678;

        [SetUp]
        public void Setup()
        {
            _query = new GetProviderAccountStatusQuery(Ukprn);
            _apiClient = new Mock<IApiClient>();
            _handler = new GetProviderAccountStatusQueryHandler(_apiClient.Object, Mock.Of<ILogger<GetProviderAccountStatusQueryHandler>>());
        }

        [Test]
        public async Task Handle_ValidApiRequest_ReturnsValidResponse()
        {
            _apiClient.Setup(x => x.Get<ProviderAccountResponse>($"providerAccounts/{_query.Ukprn}")).ReturnsAsync(_provider);
            var result = await _handler.Handle(_query, CancellationToken.None);
            result.Should().NotBeNull();
            result.CanAccessService.Should().Be(_provider.CanAccessService);
        }

        [Test]
        public async Task Handle_InvalidApiResponse_ReturnsNull()
        {
            _apiClient.Setup(x => x.Get<ProviderAccountResponse>($"providerAccounts/{_query.Ukprn}")).ReturnsAsync((ProviderAccountResponse)null);

            var result = await _handler.Handle(_query, CancellationToken.None);
            result.CanAccessService.Should().BeFalse();
        }
    }
}
