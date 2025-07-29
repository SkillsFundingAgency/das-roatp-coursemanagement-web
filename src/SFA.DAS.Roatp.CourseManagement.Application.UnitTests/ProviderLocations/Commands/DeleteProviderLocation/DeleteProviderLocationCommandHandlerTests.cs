using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.DeleteProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderLocations.Commands.DeleteProviderLocation;

[TestFixture]
public class DeleteProviderLocationCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_InvokesApiClient(
        [Frozen] Mock<IApiClient> apiClientMock,
        DeleteProviderLocationCommand request,
        DeleteProviderLocationCommandHandler sut)
    {
        var expectedUrl =
            $"providers/{request.Ukprn}/locations/{request.Id}?userId={request.UserId}&userDisplayName={request.UserDisplayName}";
        apiClientMock.Setup(a => a.Delete(expectedUrl)).ReturnsAsync(HttpStatusCode.NoContent);

        await sut.Handle(request, new CancellationToken());

        apiClientMock.Verify(a => a.Delete(expectedUrl));
    }

    [Test, MoqAutoData]
    public async Task Handle_ApiClientReturnsBadResponse_ThrowsInvalidOperationException(
        [Frozen] Mock<IApiClient> apiClientMock,
        DeleteProviderLocationCommand request,
        DeleteProviderLocationCommandHandler sut)
    {
        var expectedUrl = $"providers/{request.Ukprn}/locations/{request.Id}?userId={request.UserId}&userDisplayName={request.UserDisplayName}";
        apiClientMock.Setup(a => a.Post(expectedUrl, request)).ReturnsAsync(HttpStatusCode.BadRequest);

        Func<Task> action = () => sut.Handle(request, new CancellationToken());

        await action.Should().ThrowAsync<InvalidOperationException>();
    }
}

