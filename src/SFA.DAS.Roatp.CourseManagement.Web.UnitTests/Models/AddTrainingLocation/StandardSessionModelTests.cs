using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.AddTrainingLocation
{
    [TestFixture]
    public class StandardSessionModelTests
    {
        [Test, AutoData]
        public void Operator_TransformsLarsCode(StandardSessionModel sut)
        {
            AddProviderCourseCommand actual = sut;

            actual.LarsCode.Should().Be(sut.LarsCode);
        }

        [TestCase("Approval body", true)]
        [TestCase("", null)]
        public void Operator_IsApprovedByRegulator_IsBasedOnRegulatorName(string regulatorName, bool? expected)
        {
            var fixture = new Fixture();
            StandardSessionModel sut = fixture.Create<StandardSessionModel>();
            sut.StandardInformation.RegulatorName = regulatorName;

            AddProviderCourseCommand actual = sut;

            actual.IsApprovedByRegulator.Should().Be(expected);
        }

        [Test, AutoData]
        public void Operator_TransformsContactInformation(StandardSessionModel sut)
        {
            AddProviderCourseCommand actual = sut;

            actual.StandardInfoUrl.Should().Be(sut.ContactInformation.StandardInfoUrl);
            actual.ContactUsEmail.Should().Be(sut.ContactInformation.ContactUsEmail);
            actual.ContactUsPageUrl.Should().Be(sut.ContactInformation.ContactUsPageUrl);
            actual.ContactUsPhoneNumber.Should().Be(sut.ContactInformation.ContactUsPhoneNumber);
        }

        [TestCase(true, true)]
        [TestCase(null, false)]
        [TestCase(false, false)]
        public void Operator_TransformsHasNationalDeliveryOption(bool? actualOption, bool expectedOption)
        {
            var fixture = new Fixture();

            AddProviderCourseCommand actual = fixture.Build<StandardSessionModel>().With(s => s.HasNationalDeliveryOption, actualOption).Create();

            actual.HasNationalDeliveryOption.Should().Be(expectedOption);
        }

        [Test, AutoData]
        public void Operator_SubrerionIds_AreBasedOnCourseLocationsOfRegionType(StandardSessionModel sut, List<CourseLocationModel> regionTypeLocations)
        {
            regionTypeLocations.ForEach(l => l.LocationType = LocationType.Regional);
            sut.CourseLocations.ForEach(l => l.LocationType = LocationType.Provider);
            sut.CourseLocations.AddRange(regionTypeLocations);

            AddProviderCourseCommand actual = sut;

            actual.SubregionIds.Should().BeEquivalentTo(regionTypeLocations.Select(l => l.SubregionId));
        }

        [Test, AutoData]
        public void Operator_ProviderLocations_AreBasedOnCourseLocationsOfProviderType(StandardSessionModel sut, List<CourseLocationModel> providerTypeLocations)
        {
            providerTypeLocations.ForEach(l => l.LocationType = LocationType.Provider);
            sut.CourseLocations.ForEach(l => l.LocationType = LocationType.Regional);
            sut.CourseLocations.AddRange(providerTypeLocations);

            AddProviderCourseCommand actual = sut;

            actual.ProviderLocations.Should().BeEquivalentTo(providerTypeLocations.Select(l => (ProviderCourseLocationCommandModel) l));
        }
    }
}

