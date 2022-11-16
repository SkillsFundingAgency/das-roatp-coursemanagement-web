using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderLocations.Commands.CreateProviderLocation
{
    [TestFixture]
    public class CreateProviderLocationCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_InvokesApiClient(
            [Frozen] Mock<IApiClient> apiClientMock,
            CreateProviderLocationCommandHandler sut,
            CreateProviderLocationCommand command)
        {
            apiClientMock.Setup(a => a.Post($"providers/{command.Ukprn}/locations/create-provider-location", command)).ReturnsAsync(HttpStatusCode.Created);
            
            await sut.Handle(command, new CancellationToken());

            apiClientMock.Verify(a => a.Post($"providers/{command.Ukprn}/locations/create-provider-location", It.Is<CreateProviderLocationCommand>(r => r.Ukprn == command.Ukprn && r.UserId == command.UserId &&  r.UserDisplayName == command.UserDisplayName && r.LocationName == command.LocationName && r.AddressLine1 == command.AddressLine1 && r.AddressLine2 == command.AddressLine2 && r.Town == command.Town && r.Postcode == command.Postcode && r.County == command.County && r.Latitude == command.Latitude && r.Longitude == command.Longitude && r.Email == command.Email && r.Website == command.Website && r.Phone == command.Phone)));
        }

        [Test, MoqAutoData]
        public async Task Handle_ApiClientDoesNotReturnCreatedStatus_ThrowsException(
            [Frozen] Mock<IApiClient> apiClientMock,
            CreateProviderLocationCommandHandler sut,
            CreateProviderLocationCommand command)
        {
            apiClientMock.Setup(a => a.Post($"providers/{command.Ukprn}/locations/create-provider-location", command)).ReturnsAsync(HttpStatusCode.BadRequest);

            Func<Task> action = () => sut.Handle(command, new CancellationToken());

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
