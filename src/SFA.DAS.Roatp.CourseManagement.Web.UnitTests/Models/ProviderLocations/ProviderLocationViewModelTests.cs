using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ProviderLocations
{
    [TestFixture]
    public class ProviderLocationViewModelTests
    {
        [Test]
        public void ImplicitOperater_ConvertsFromProviderLocation()
        {
            var providerLocation = new ProviderLocation();

            ProviderLocationViewModel viewModel = providerLocation;

            viewModel.Should().BeEquivalentTo(providerLocation, o =>
            {
                o.Excluding(c => c.LocationType);
                o.Excluding(c => c.Email);
                o.Excluding(c => c.Phone);
                return o;
            });
        }
    }
}
