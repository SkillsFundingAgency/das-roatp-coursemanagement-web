using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Standards.Queries.GetStandardInformation
{
    [TestFixture]
    public class GetStandardInformationQueryHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_ApiCallSuccessful_ReturnsResult(
            [Frozen] Mock<IApiClient> apiClientMock,
            GetStandardInformationQueryHandler sut,
            GetStandardInformationQuery request,
            GetStandardInformationQueryResult expectedResult)
        {
            apiClientMock.Setup(c => c.Get<GetStandardInformationQueryResult>($"lookup/standards/{request.LarsCode}")).ReturnsAsync(expectedResult);

            var actualResult = await sut.Handle(request, new CancellationToken());

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test, MoqAutoData]
        public async Task Handle_ApiCallFails_ThrowsException(
            [Frozen] Mock<IApiClient> apiClientMock,
            GetStandardInformationQueryHandler sut,
            GetStandardInformationQuery request)
        {
            apiClientMock.Setup(c => c.Get<GetStandardInformationQueryResult>($"lookup/standards/{request.LarsCode}")).ReturnsAsync((GetStandardInformationQueryResult) null);

            Func<Task> action = () => sut.Handle(request, new CancellationToken());

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
