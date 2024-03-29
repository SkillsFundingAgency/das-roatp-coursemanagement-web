﻿using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Commands.UpdateStandardSubRegions
{
    [TestFixture]
    public class UpdateStandardSubRegionsCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_OuterApiCallSuccess_ReturnsUnit(
            [Frozen] Mock<IApiClient> apiClientMock,
            UpdateStandardSubRegionsCommandHandler sut,
            UpdateStandardSubRegionsCommand command,
            CancellationToken cancellationToken)
        {
            var expectedUri = $"providers/{command.Ukprn}/courses/{command.LarsCode}/locations/regions";
            apiClientMock.Setup(c => c.Post(expectedUri, command)).ReturnsAsync(HttpStatusCode.NoContent);

            var result = await sut.Handle(command, cancellationToken);

            apiClientMock.Verify(c => c.Post(expectedUri, command));
            result.Should().BeEquivalentTo(Unit.Value);
        }

        [Test, MoqAutoData]
        public async Task Handle_OuterApiCallFails_ThrowsInvalidOperationException(
            [Frozen] Mock<IApiClient> apiClientMock,
            UpdateStandardSubRegionsCommandHandler sut,
            UpdateStandardSubRegionsCommand command,
            CancellationToken cancellationToken)
        {
            var expectedUri = $"providers/{command.Ukprn}/courses/{command.LarsCode}/locations/regions";
            apiClientMock.Setup(c => c.Post(expectedUri, command)).ReturnsAsync(HttpStatusCode.BadRequest);

            Func<Task> act = () => sut.Handle(command, cancellationToken);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
