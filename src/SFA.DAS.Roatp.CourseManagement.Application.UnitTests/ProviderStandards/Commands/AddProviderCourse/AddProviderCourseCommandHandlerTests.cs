using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Commands.AddProviderCourse
{
    [TestFixture]
    public class AddProviderCourseCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_InvokesApiClient(
            [Frozen] Mock<IApiClient> apiClientMock,
            AddProviderCourseCommandHandler sut,
            AddProviderCourseCommand command)
        {
            apiClientMock.Setup(a => a.Post($"providers/{command.Ukprn}/courses/{command.LarsCode}/create", command)).ReturnsAsync(HttpStatusCode.Created);

            await sut.Handle(command, new CancellationToken());

            apiClientMock.Verify(a => a.Post($"providers/{command.Ukprn}/courses/{command.LarsCode}/create", command));
        }

        [Test, MoqAutoData]
        public async Task Handle_ApiClientDoesNotReturnCreatedStatus_ThrowsException(
            [Frozen] Mock<IApiClient> apiClientMock,
            AddProviderCourseCommandHandler sut,
            AddProviderCourseCommand command)
        {
            apiClientMock.Setup(a => a.Post($"providers/{command.Ukprn}/courses/{command.LarsCode}/create", command)).ReturnsAsync(HttpStatusCode.BadRequest);

            Func<Task> action = () => sut.Handle(command, new CancellationToken());

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
