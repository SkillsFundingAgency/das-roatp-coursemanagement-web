using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Constants;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ProviderCourseLocations
{
    [TestFixture]
    public class ProviderCourseLocationViewModelTests
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
            var hasDayReleaseDeliveryOption = true;
            var providerCourseLocation = new ProviderCourseLocation
            {
                HasDayReleaseDeliveryOption = hasDayReleaseDeliveryOption,
            };
        
            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.HasDayReleaseDeliveryOption.Should().Be(hasDayReleaseDeliveryOption);
            viewModel.DeliveryOption().Should().Be(CourseLocationDeliveryOption.DayRelease);
        }

        [Test]
        public void ImplicitOperator_ViewModelShouldReturnBlockReleaseDeliveryOption()
        {
            var hasBlockReleaseDeliveryOption = true;
            var providerCourseLocation = new ProviderCourseLocation
            {
                HasBlockReleaseDeliveryOption = hasBlockReleaseDeliveryOption,
            };
        
            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.HasBlockReleaseDeliveryOption.Should().Be(hasBlockReleaseDeliveryOption);
            viewModel.DeliveryOption().Should().Be(CourseLocationDeliveryOption.BlockRelease);
        }

        [Test]
        public void ImplicitOperator_ViewModelShouldReturnDayAndBlockReleaseDeliveryOption()
        {
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
            viewModel.DeliveryOption().Should().Be(CourseLocationDeliveryOption.DayAndBlockRelease);
        }

        [Test]
        public void ImplicitOperator_ViewModelShouldReturnNoDeliveryOption()
        {
            var providerCourseLocation = new ProviderCourseLocation();
            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.DeliveryOption().Should().Be(CourseLocationDeliveryOption.NotSet);
        }
    }
}