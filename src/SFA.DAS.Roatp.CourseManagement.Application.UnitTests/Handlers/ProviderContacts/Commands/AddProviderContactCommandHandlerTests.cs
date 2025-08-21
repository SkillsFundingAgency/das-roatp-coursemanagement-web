using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderContact;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers.ProviderContacts.Commands;

[TestFixture]
public class AddProviderContactCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_InvokesApiClient(
        [Frozen] Mock<IApiClient> apiClientMock,
        AddProviderContactCommandHandler sut,
        AddProviderContactCommand command)
    {
        apiClientMock.Setup(a => a.Post($"providers/{command.Ukprn}/contact", command)).ReturnsAsync(HttpStatusCode.Created);

        await sut.Handle(command, new CancellationToken());

        apiClientMock.Verify(a => a.Post($"providers/{command.Ukprn}/contact", It.Is<AddProviderContactCommand>(r => r.Ukprn == command.Ukprn && r.UserId == command.UserId && r.UserDisplayName == command.UserDisplayName && r.EmailAddress == command.EmailAddress && r.PhoneNumber == command.PhoneNumber && r.ProviderCourseIds.Count == command.ProviderCourseIds.Count)));
    }

    [Test, MoqAutoData]
    public async Task Handle_ApiClientDoesNotReturnCreatedStatus_ThrowsException(
        [Frozen] Mock<IApiClient> apiClientMock,
        AddProviderContactCommandHandler sut,
        AddProviderContactCommand command)
    {
        apiClientMock.Setup(a => a.Post($"providers/{command.Ukprn}/contact", command)).ReturnsAsync(HttpStatusCode.BadRequest);

        Func<Task> action = () => sut.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<InvalidOperationException>();
    }
}
