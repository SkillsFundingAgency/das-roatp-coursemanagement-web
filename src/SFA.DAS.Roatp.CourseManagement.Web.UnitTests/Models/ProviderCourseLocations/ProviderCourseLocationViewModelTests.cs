using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ProviderCourseLocations
{
    [TestFixture]
    class ProviderCourseLocationViewModelTests
    {
        [TestCase(LocationType.Provider, true, true, true)]
        [TestCase(LocationType.Regional, true, true, true)]
        [TestCase(LocationType.National, true, true, true)]
        [TestCase(LocationType.Provider, true, false, true)]
        [TestCase(LocationType.Regional, true, false, true)]
        [TestCase(LocationType.National, true, false, true)]
        [TestCase(LocationType.Provider, true, false, false)]
        [TestCase(LocationType.Regional, true, false, false)]
        [TestCase(LocationType.National, true, false, false)]
        public void ImplicitOperator_ConvertsFromProviderCourseLocation(LocationType locationType, bool hasBlockReleaseDeliveryOption, bool hasDayReleaseDeliveryOption, bool offersPortableFlexiJob)
        {
            const string _locationName = "Test location";
            LocationType _locationType = locationType;
            var providerCourseLocation = new ProviderCourseLocation 
            {   
                LocationName = _locationName, 
                LocationType = _locationType, 
                HasBlockReleaseDeliveryOption = hasBlockReleaseDeliveryOption, 
                HasDayReleaseDeliveryOption = hasDayReleaseDeliveryOption, 
                OffersPortableFlexiJob = offersPortableFlexiJob
            };

            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.LocationName.Should().Be(_locationName);
            viewModel.LocationType.Should().Be(_locationType);
            viewModel.HasBlockReleaseDeliveryOption.Should().Be(hasBlockReleaseDeliveryOption);
            viewModel.HasDayReleaseDeliveryOption.Should().Be(hasDayReleaseDeliveryOption);
            viewModel.OffersPortableFlexiJob.Should().Be(offersPortableFlexiJob);
        }
    }
}
