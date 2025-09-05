using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers.ProviderContacts.Queries;

[TestFixture]
public class GetLatestProviderContactQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_InvokesApiClient_ReturnsExpectedResponse(
        [Frozen] Mock<IApiClient> apiClientMock,
        GetLatestProviderContactQueryHandler sut,
        GetLatestProviderContactQuery request,
        ProviderContactModel returnedModel)
    {
        apiClientMock.Setup(a => a.Get<ProviderContactModel>($"providers/{request.Ukprn}/contact")).ReturnsAsync(returnedModel);

        var response = await sut.Handle(request, new CancellationToken());

        apiClientMock.Verify(a => a.Get<ProviderContactModel>($"providers/{request.Ukprn}/contact"), Times.Once);
        response.EmailAddress.Should().Be(returnedModel.EmailAddress);
        response.PhoneNumber.Should().Be(returnedModel.PhoneNumber);
    }

    [Test, MoqAutoData]
    public async Task Handle_InvokesApiClient_NullClientResonse_NullReturned(
        [Frozen] Mock<IApiClient> apiClientMock,
        GetLatestProviderContactQueryHandler sut,
        GetLatestProviderContactQuery request,
        ProviderContactModel returnedModel)
    {
        apiClientMock.Setup(a => a.Get<ProviderContactModel>($"providers/{request.Ukprn}/contact")).ReturnsAsync((ProviderContactModel)null);

        var response = await sut.Handle(request, new CancellationToken());

        apiClientMock.Verify(a => a.Get<ProviderContactModel>($"providers/{request.Ukprn}/contact"), Times.Once);
        response.Should().BeNull();
    }
}
