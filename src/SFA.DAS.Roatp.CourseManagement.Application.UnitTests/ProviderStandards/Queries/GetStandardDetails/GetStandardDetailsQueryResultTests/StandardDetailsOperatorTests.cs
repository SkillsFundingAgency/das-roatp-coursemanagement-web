using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Queries.GetStandardDetails.GetStandardDetailsQueryResultTests
{
    [TestFixture]
    public class StandardDetailsOperatorTests
    {
        [Test, AutoData]
        public void StandardDetailsOperator_ReturnsResult(StandardDetails source)
        {
            GetStandardDetailsQueryResult result = source;
            Assert.IsNotNull(result);
            result.Should().BeEquivalentTo(source, opt =>
            {
                opt.Including(s => s.ContactUsEmail);
                opt.Including(s => s.ContactUsPageUrl);
                opt.Including(s => s.ContactUsPhoneNumber);
                opt.Including(s => s.IFateReferenceNumber);
                opt.Including(s => s.LarsCode);
                opt.Including(s => s.Level);
                opt.Including(s => s.ProviderCourseLocations);
                opt.Including(s => s.RegulatorName);
                opt.Including(s => s.Sector);
                opt.Including(s => s.StandardInfoUrl);
                opt.Including(s => s.ApprenticeshipType);
                opt.Including(s => s.IsApprovedByRegulator);
                return opt;
            });
        }
    }
}
