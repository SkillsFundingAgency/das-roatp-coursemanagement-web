using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ProviderCourseLocations
{
    [TestFixture]
    public class ProviderCourseLocationViewModelTests
    {
        [TestCase(LocationType.Provider, true, true)]
        [TestCase(LocationType.Regional, true, true)]
        [TestCase(LocationType.National, true, true)]
        [TestCase(LocationType.Provider, true, false)]
        [TestCase(LocationType.Regional, true, false)]
        [TestCase(LocationType.National, true, false)]
        [TestCase(LocationType.Provider, true, false)]
        [TestCase(LocationType.Regional, true, false)]
        [TestCase(LocationType.National, true, false)]
        public void ImplicitOperator_ConvertsFromProviderCourseLocation(LocationType locationType, bool hasBlockReleaseDeliveryOption, bool hasDayReleaseDeliveryOption)
        {
            const string locationName = "Test location";
            LocationType _locationType = locationType;
            var providerCourseLocation = new ProviderCourseLocation
            {
                LocationName = locationName,
                LocationType = _locationType,
                HasBlockReleaseDeliveryOption = hasBlockReleaseDeliveryOption,
                HasDayReleaseDeliveryOption = hasDayReleaseDeliveryOption
            };

            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.LocationName.Should().Be(locationName);
            viewModel.LocationType.Should().Be(_locationType);
            viewModel.DeliveryMethod.HasBlockReleaseDeliveryOption.Should().Be(hasBlockReleaseDeliveryOption);
            viewModel.DeliveryMethod.HasDayReleaseDeliveryOption.Should().Be(hasDayReleaseDeliveryOption);
        }

        [Test]
        public void ImplicitOperator_ViewModelShouldReturnDayReleaseDeliveryOption()
        {
            const bool hasDayReleaseDeliveryOption = true;
            var providerCourseLocation = new ProviderCourseLocation
            {
                HasDayReleaseDeliveryOption = hasDayReleaseDeliveryOption,
            };

            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.DeliveryMethod.HasDayReleaseDeliveryOption.Should().Be(hasDayReleaseDeliveryOption);
            viewModel.DeliveryMethod.ToSummary().Should().Be(DeliveryMethodModel.DayRelease);
        }

        [Test]
        public void ImplicitOperator_ViewModelShouldReturnBlockReleaseDeliveryOption()
        {
            const bool hasBlockReleaseDeliveryOption = true;
            var providerCourseLocation = new ProviderCourseLocation
            {
                HasBlockReleaseDeliveryOption = hasBlockReleaseDeliveryOption,
            };

            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.DeliveryMethod.HasBlockReleaseDeliveryOption.Should().Be(hasBlockReleaseDeliveryOption);
            viewModel.DeliveryMethod.ToSummary().Should().Be(DeliveryMethodModel.BlockRelease);
        }

        [Test]
        public void ImplicitOperator_ViewModelShouldReturnDayAndBlockReleaseDeliveryOption()
        {
            const bool hasDayReleaseDeliveryOption = true;
            const bool hasBlockReleaseDeliveryOption = true;
            var providerCourseLocation = new ProviderCourseLocation
            {
                HasDayReleaseDeliveryOption = hasDayReleaseDeliveryOption,
                HasBlockReleaseDeliveryOption = hasBlockReleaseDeliveryOption,
            };

            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.DeliveryMethod.HasBlockReleaseDeliveryOption.Should().Be(hasBlockReleaseDeliveryOption);
            viewModel.DeliveryMethod.HasDayReleaseDeliveryOption.Should().Be(hasDayReleaseDeliveryOption);
            viewModel.DeliveryMethod.ToSummary().Should().Be(DeliveryMethodModel.DayAndBlockRelease);
        }

        [Test]
        public void ImplicitOperator_ViewModelShouldReturnNoDeliveryOption()
        {
            var providerCourseLocation = new ProviderCourseLocation();
            ProviderCourseLocationViewModel viewModel = providerCourseLocation;
            viewModel.DeliveryMethod.ToSummary().Should().Be(string.Empty);
        }
    }
}