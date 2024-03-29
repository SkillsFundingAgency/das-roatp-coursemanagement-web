﻿using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    [TestFixture]
    public class RegionViewModelTests
    {
        [Test, AutoData]
        public void CourseRegionModelOperator_ReturnsViewModel(CourseRegionModel source)
        {
            var vm = (RegionViewModel)source;

            vm.Should().BeEquivalentTo(source);
        }

        [Test, AutoData]
        public void RegionModelOperator_ReturnsViewModel(RegionModel source)
        {
            var vm = (RegionViewModel)source;

            vm.Should().BeEquivalentTo(source);
        }
    }
}
