using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAddresses;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderLocations.Queries.GetAddresses
{
    [TestFixture]
    public class GetAddressesQueryHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_InvokesApiClient(
            [Frozen] Mock<IApiClient> apiClientMock,
            GetAddressesQueryHandler sut,
            GetAddressesQuery request)
        {
            await sut.Handle(request, new CancellationToken());

            apiClientMock.Verify(a => a.Get<GetAddressesQueryResult>($"lookup/addresses?postcode={HttpUtility.UrlEncode(request.Postcode)}"));
        }
    }
}
