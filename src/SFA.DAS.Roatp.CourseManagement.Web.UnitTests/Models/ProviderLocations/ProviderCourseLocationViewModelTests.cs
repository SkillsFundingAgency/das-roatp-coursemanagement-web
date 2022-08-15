using AutoFixture.NUnit3;
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
        [Test, AutoData]
        public void ImplicitOperator_ConvertsApiModelToViewModel(ProviderCourseLocation providerCourseLocation)
        { 
            ProviderCourseLocationViewModel viewModel = providerCourseLocation;

            viewModel.LocationName.Should().Be(providerCourseLocation.LocationName);
            viewModel.LocationType.Should().Be(providerCourseLocation.LocationType);
            viewModel.DeliveryMethod.HasBlockReleaseDeliveryOption.Should().Be(providerCourseLocation.HasBlockReleaseDeliveryOption);
            viewModel.DeliveryMethod.HasDayReleaseDeliveryOption.Should().Be(providerCourseLocation.HasDayReleaseDeliveryOption);
            viewModel.RegionName.Should().Be(providerCourseLocation.RegionName);
            viewModel.SubregionName.Should().Be(providerCourseLocation.SubregionName);
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
            viewModel.DeliveryMethod.ToSummary().Should().Be(DeliveryMethodModel.DayReleaseDescription);
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
            viewModel.DeliveryMethod.ToSummary().Should().Be(DeliveryMethodModel.BlockReleaseDescription);
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
            viewModel.DeliveryMethod.ToSummary().Should().Be(DeliveryMethodModel.DayAndBlockReleaseDescription);
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