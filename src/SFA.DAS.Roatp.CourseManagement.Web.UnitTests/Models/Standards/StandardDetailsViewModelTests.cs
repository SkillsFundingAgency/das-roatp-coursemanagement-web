using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    [TestFixture]
    public class StandardDetailsViewModelTests
    {
        [TestCase("regulator name",true)]
        [TestCase("",false)]
        public void ImplicitOperator_ConvertsFromStandardDetails(string regulatorName, bool isRegulated)
        {
            const string courseName = "course name";
            const string level = "2";
            const string iFateReferenceNumber = "STD_1";
            const string sector = "digital";
            const int larsCode = 133;
            const string version = "3";
            var expectedCourseDisplayName = $"{courseName} (Level {level})";
            var providerCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation { LocationName ="Test", LocationType = LocationType.Provider, HasBlockReleaseDeliveryOption = true, HasDayReleaseDeliveryOption= true, OffersPortableFlexiJob= true}
            };

            var standardDetails = new StandardDetails
            {
                CourseName = courseName,
                LarsCode = larsCode,
                Level = level,
                IFateReferenceNumber = iFateReferenceNumber,
                Sector = sector,
                Version = version,
                RegulatorName = regulatorName,
                ProviderCourseLocations = providerCourseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;
            viewModel.CourseName.Should().Be(courseName);
            viewModel.Level.Should().Be(level);
            viewModel.IFateReferenceNumber.Should().Be(iFateReferenceNumber);
            viewModel.Sector.Should().Be(sector);
            viewModel.LarsCode.Should().Be(larsCode);
            viewModel.Version.Should().Be(version);
            viewModel.RegulatorName.Should().Be(regulatorName);
            viewModel.IsStandardRegulated.Should().Be(isRegulated);
            viewModel.CourseDisplayName.Should().Be(expectedCourseDisplayName);
            viewModel.ProviderCourseLocations.Should().BeEquivalentTo(providerCourseLocations);
        }
    }
}
