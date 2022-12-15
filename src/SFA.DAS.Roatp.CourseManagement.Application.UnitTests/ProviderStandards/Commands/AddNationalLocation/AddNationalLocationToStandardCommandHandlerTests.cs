using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddNationalLocation;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Commands.AddNationalLocation
{
    [TestFixture]
    public class AddNationalLocationToStandardCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_InvokesApiClient(
                    [Frozen] Mock<IApiClient> apiClientMock,
                    AddNationalLocationToStandardCommand request,
                    AddNationalLocationToStandardCommandHandler sut)
        {
            var expectedUrl = $"providers/{request.Ukprn}/courses/{request.LarsCode}/locations/national";
            apiClientMock.Setup(a => a.Post(expectedUrl, request)).ReturnsAsync(HttpStatusCode.NoContent);

            await sut.Handle(request, new CancellationToken());

            apiClientMock.Verify(a => a.Post(expectedUrl, request));
        }

        [Test, MoqAutoData]
        public async Task Handle_ApiClientReturnsBadResponse_ThrowsInvalidOperationException(
            [Frozen] Mock<IApiClient> apiClientMock,
            AddNationalLocationToStandardCommand request,
            AddNationalLocationToStandardCommandHandler sut)
        {
            var expectedUrl = $"providers/{request.Ukprn}/courses/{request.LarsCode}/bulk-delete-course-locations";
            apiClientMock.Setup(a => a.Post(expectedUrl, request)).ReturnsAsync(HttpStatusCode.BadRequest);

            Func<Task<Unit>> action = () => sut.Handle(request, new CancellationToken());

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
