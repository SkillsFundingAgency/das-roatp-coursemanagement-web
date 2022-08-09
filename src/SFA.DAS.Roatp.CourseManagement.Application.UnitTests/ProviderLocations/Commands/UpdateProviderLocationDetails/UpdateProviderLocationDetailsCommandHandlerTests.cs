using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.UpdateProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderLocations.Commands.CreateProviderLocation
{
    [TestFixture]
    public class UpdateProviderLocationDetailsCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_InvokesApiClient(
            [Frozen] Mock<IApiClient> apiClientMock,
            UpdateProviderLocationDetailsCommandHandler sut,
            UpdateProviderLocationDetailsCommand command)
        {
            apiClientMock.Setup(a => a.Post($"providers/{command.Ukprn}/locations/{command.Id}/update-provider-location-details", command)).ReturnsAsync(HttpStatusCode.NoContent);

            await sut.Handle(command, new CancellationToken());

            apiClientMock.Verify(a => a.Post($"providers/{command.Ukprn}/locations/{command.Id}/update-provider-location-details", command));
        }

        [Test, MoqAutoData]
        public async Task Handle_ApiClientDoesNotReturnCreatedStatus_ThrowsException(
            [Frozen] Mock<IApiClient> apiClientMock,
            UpdateProviderLocationDetailsCommandHandler sut,
            UpdateProviderLocationDetailsCommand command)
        {
            apiClientMock.Setup(a => a.Post($"providers/{command.Ukprn}/locations/{command.Id}/update-provider-location-details", command)).ReturnsAsync(HttpStatusCode.BadRequest);

            Func<Task> action = () => sut.Handle(command, new CancellationToken());

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
