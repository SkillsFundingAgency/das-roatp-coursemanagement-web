using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Commands.AddProviderCourseLocation
{
    [TestFixture]
    public class AddProviderCourseLocationCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_InvokesApiClient(
            [Frozen] Mock<IApiClient> apiClientMock,
            AddProviderCourseLocationCommand request,
            AddProviderCourseLocationCommandHandler sut)
        {
            var expectedUrl = $"providers/{request.Ukprn}/courses/{request.LarsCode}/create-providercourselocation";
            apiClientMock.Setup(a => a.Post(expectedUrl, request)).ReturnsAsync(HttpStatusCode.Created);

            await sut.Handle(request, new CancellationToken());

            apiClientMock.Verify(a => a.Post(expectedUrl, It.Is<AddProviderCourseLocationCommand>(r => r.Ukprn == request.Ukprn && r.LarsCode == request.LarsCode && r.UserId == request.UserId && r.UserDisplayName == request.UserDisplayName && r.LocationNavigationId == request.LocationNavigationId && r.HasDayReleaseDeliveryOption == request.HasDayReleaseDeliveryOption && r.HasBlockReleaseDeliveryOption == request.HasBlockReleaseDeliveryOption)));
        }

        [Test, MoqAutoData]
        public async Task Handle_ApiClientReturnsBadResponse_ThrowsInvalidOperationException(
            [Frozen] Mock<IApiClient> apiClientMock,
            AddProviderCourseLocationCommand request,
            AddProviderCourseLocationCommandHandler sut)
        {
            var expectedUrl = $"providers/{request.Ukprn}/courses/{request.LarsCode}/create-providercourselocation";
            apiClientMock.Setup(a => a.Post(expectedUrl, request)).ReturnsAsync(HttpStatusCode.BadRequest);

            Func<Task<Unit>> action = () => sut.Handle(request, new CancellationToken());

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
