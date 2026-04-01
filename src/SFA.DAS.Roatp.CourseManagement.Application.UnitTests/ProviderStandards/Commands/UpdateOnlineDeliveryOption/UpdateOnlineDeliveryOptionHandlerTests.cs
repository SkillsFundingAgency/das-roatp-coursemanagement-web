using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateOnlineDeliveryOption;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Commands.UpdateOnlineDeliveryOption;
public class UpdateOnlineDeliveryOptionHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_OuterApiCallSuccess_ReturnsUnit(
            [Frozen] Mock<IApiClient> apiClientMock,
            UpdateOnlineDeliveryOptionCommandHandler sut,
            UpdateOnlineDeliveryOptionCommand command,
            CancellationToken cancellationToken)
    {
        var expectedUri = $"providers/{command.Ukprn}/courses/{command.LarsCode}/update-online-delivery-option";
        apiClientMock.Setup(c => c.Post(expectedUri, It.IsAny<object>())).ReturnsAsync(HttpStatusCode.NoContent);

        var result = await sut.Handle(command, cancellationToken);

        apiClientMock.Verify(c => c.Post(expectedUri, It.IsAny<object>()));
        result.Should().BeEquivalentTo(Unit.Value);
    }

    [Test, MoqAutoData]
    public async Task Handle_OuterApiCallFails_ThrowsInvalidOperationException(
        [Frozen] Mock<IApiClient> apiClientMock,
        UpdateOnlineDeliveryOptionCommandHandler sut,
        UpdateOnlineDeliveryOptionCommand command,
        CancellationToken cancellationToken)
    {
        var expectedUri = $"providers/{command.Ukprn}/courses/{command.LarsCode}/update-online-delivery-option";
        apiClientMock.Setup(c => c.Post(expectedUri, It.IsAny<object>())).ReturnsAsync(HttpStatusCode.BadRequest);

        Func<Task> act = () => sut.Handle(command, cancellationToken);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
