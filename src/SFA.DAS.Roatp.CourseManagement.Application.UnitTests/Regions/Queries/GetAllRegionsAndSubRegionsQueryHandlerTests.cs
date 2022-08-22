using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Regions.Queries
{
    [TestFixture]
    public class GetAllRegionsAndSubRegionsQueryHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_NoData_ThrowsException(
            [Frozen] Mock<IApiClient> apiClientMock,
            GetAllRegionsAndSubRegionsQueryHandler sut,
            GetAllRegionsAndSubRegionsQuery request)
        {
            apiClientMock.Setup(c => c.Get<GetAllRegionsAndSubRegionsQueryResult>(GetAllRegionsAndSubRegionsQueryHandler.LookupRegionsRoute)).ReturnsAsync((GetAllRegionsAndSubRegionsQueryResult) null);

            Func<Task> action = () => sut.Handle(request, new CancellationToken());

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test, MoqAutoData]
        public async Task Handle_ReturnsRegions(
            [Frozen] Mock<IApiClient> apiClientMock,
            GetAllRegionsAndSubRegionsQueryHandler sut,
            GetAllRegionsAndSubRegionsQuery request,
            GetAllRegionsAndSubRegionsQueryResult queryResult)
        {
            apiClientMock.Setup(c => c.Get<GetAllRegionsAndSubRegionsQueryResult>(GetAllRegionsAndSubRegionsQueryHandler.LookupRegionsRoute)).ReturnsAsync(queryResult);

            var result = await sut.Handle(request, new CancellationToken());

            result.Regions.Should().BeEquivalentTo(queryResult.Regions, options => options.ExcludingMissingMembers());
        }

    }
}
