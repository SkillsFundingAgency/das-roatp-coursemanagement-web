using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Commands.DeleteProviderCourseLocation
{
    [TestFixture]
    public class DeleteProviderCourseLocationCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_InvokesApiClient(
            [Frozen] Mock<IApiClient> apiClientMock,
            DeleteProviderCourseLocationCommand request,
            DeleteProviderCourseLocationCommandHandler sut)
        {
            var expectedUrl = $"providers/{request.Ukprn}/courses/{request.LarsCode}/location/{request.Id}/delete";
            apiClientMock.Setup(a => a.Post(expectedUrl, request)).ReturnsAsync(HttpStatusCode.NoContent);

            await sut.Handle(request, new CancellationToken());

            apiClientMock.Verify(a => a.Post(expectedUrl, request));
        }

        [Test, MoqAutoData]
        public async Task Handle_ApiClientReturnsBadResponse_ThrowsInvalidOperationException(
            [Frozen] Mock<IApiClient> apiClientMock,
            DeleteProviderCourseLocationCommand request,
            DeleteProviderCourseLocationCommandHandler sut)
        {
            var expectedUrl = $"providers/{request.Ukprn}/courses/{request.LarsCode}/location/{request.Id}/delete";
            apiClientMock.Setup(a => a.Post(expectedUrl, request)).ReturnsAsync(HttpStatusCode.BadRequest);

            Func<Task<Unit>> action = () => sut.Handle(request, new CancellationToken());

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
