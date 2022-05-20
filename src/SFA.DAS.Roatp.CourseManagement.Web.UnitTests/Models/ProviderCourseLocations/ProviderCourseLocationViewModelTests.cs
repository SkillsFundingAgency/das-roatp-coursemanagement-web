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

        [Test]
        public void ImplicitOperator_ViewModelShouldReturnDayReleaseDeliveryOption()
        {
            const string expectedDeliveryOptionDayRelease = "Day release";
            bool hasDayReleaseDeliveryOption = true;
            var providerCourseLocation = new ProviderCourseLocation
            {
                HasDayReleaseDeliveryOption = hasDayReleaseDeliveryOption,
            };

            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.HasDayReleaseDeliveryOption.Should().Be(hasDayReleaseDeliveryOption);
            viewModel.DeliveryOption().Should().Be(expectedDeliveryOptionDayRelease);
        }

        [Test]
        public void ImplicitOperator_ViewModelShouldReturnBlockReleaseDeliveryOption()
        {
            const string expectedDeliveryOptionBlockRelease = "Block release";
            bool hasBlockReleaseDeliveryOption = true;
            var providerCourseLocation = new ProviderCourseLocation
            {
                HasBlockReleaseDeliveryOption = hasBlockReleaseDeliveryOption,
            };

            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.HasBlockReleaseDeliveryOption.Should().Be(hasBlockReleaseDeliveryOption);
            viewModel.DeliveryOption().Should().Be(expectedDeliveryOptionBlockRelease);
        }

        [Test]
        public void ImplicitOperator_ViewModelShouldReturnDayAndBlockReleaseDeliveryOption()
        {
            const string expectedDeliveryOptionBlockAndDayRelease = "Day & block release";
            bool hasDayReleaseDeliveryOption = true;
            bool hasBlockReleaseDeliveryOption = true;
            var providerCourseLocation = new ProviderCourseLocation
            {
                HasDayReleaseDeliveryOption = hasDayReleaseDeliveryOption,
                HasBlockReleaseDeliveryOption = hasBlockReleaseDeliveryOption,
            };

            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.HasBlockReleaseDeliveryOption.Should().Be(hasBlockReleaseDeliveryOption);
            viewModel.HasDayReleaseDeliveryOption.Should().Be(hasDayReleaseDeliveryOption);
            viewModel.DeliveryOption().Should().Be(expectedDeliveryOptionBlockAndDayRelease);
        }

        [Test]
        public void ImplicitOperator_ViewModelShouldReturnNoDeliveryOption()
        {
            var providerCourseLocation = new ProviderCourseLocation();
            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.DeliveryOption().Should().BeEmpty();
        }

        [TestCase(true, "Yes")]
        [TestCase(false, "No")]
        public void ImplicitOperator_ViewModelShouldReturnDayAndBlockReleaseDeliveryOption(bool offersPortableFlexiJob, string displayText)
        {
            var providerCourseLocation = new ProviderCourseLocation
            {
                OffersPortableFlexiJob = offersPortableFlexiJob
            };

            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.OffersPortableFlexiJob.Should().Be(offersPortableFlexiJob);
            viewModel.HasOffersPortableFlexiJob.Should().Be(displayText);
        }
    }
}
