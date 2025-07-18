using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetailsForDeletion;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Queries.GetProviderLocationDetailsForDeletion;

[TestFixture]
public class GetProviderLocationDetailsForDeletionQueryHandlerTests
{
    private readonly ProviderLocation _providerLocation = new() { LocationName = "ABC Training", LocationType = LocationType.Provider };

    private GetProviderLocationDetailsForDeletionQueryHandler _handler;
    private Mock<IApiClient> _apiClient;
    private GetProviderLocationDetailsForDeletionQuery _query;
    private GetProviderLocationDetailsQueryResult _result;

    [SetUp]
    public void Setup()
    {
        var autoFixture = new Fixture();

        _query = autoFixture.Create<GetProviderLocationDetailsForDeletionQuery>();
        _result = autoFixture.Create<GetProviderLocationDetailsQueryResult>();
        _apiClient = new Mock<IApiClient>();
        _handler = new GetProviderLocationDetailsForDeletionQueryHandler(_apiClient.Object, Mock.Of<ILogger<GetProviderLocationDetailsForDeletionQueryHandler>>());
    }

    [Test]
    public async Task Handle_ValidApiRequest_ReturnsValidResponse()
    {
        _apiClient.Setup(x => x.Get<ProviderLocation>($"providers/{_query.Ukprn}/locations/{_query.Id}")).ReturnsAsync(_providerLocation);

        var result = await _handler.Handle(_query, CancellationToken.None);

        result.Should().NotBeNull();
        result.ProviderLocation.Should().BeEquivalentTo(_providerLocation);
    }

    [Test]
    public async Task Handle_ValidApiResponse_ReturnsNullResponse()
    {
        _apiClient.Setup(x => x.Get<ProviderLocation>($"providers/{_query.Ukprn}/locations/{_query.Id}")).ReturnsAsync((ProviderLocation)null);

        var result = await _handler.Handle(_query, CancellationToken.None);

        result.Should().BeNull();
    }
}
