using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseType.Queries.GetProviderCourseType;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderCourseType.Queries.GetProviderCourseType;
public class GetProviderCourseTypeQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handler_CallsApiClient_ReturnsResult(
            [Frozen] Mock<IApiClient> apiClientMock,
            List<CourseTypeModel> expectedResult,
            GetProviderCourseTypeQuery request,
            GetProviderCourseTypeQueryHandler sut)
    {
        apiClientMock.Setup(a => a.Get<List<CourseTypeModel>>($"providers/{request.Ukprn}/course-types")).ReturnsAsync(expectedResult);

        var actualResult = await sut.Handle(request, new CancellationToken());

        apiClientMock.Verify(x => x.Get<List<CourseTypeModel>>($"providers/{request.Ukprn}/course-types"), Times.Once);
        expectedResult.Should().BeEquivalentTo(actualResult);
    }
}
