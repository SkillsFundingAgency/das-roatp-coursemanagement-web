using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    [TestFixture]
    public class StandardInformationViewModelTests
    {
        [Test, AutoData]
        public void Operator_TransformsFromGetStandardInformationQueryResult(GetStandardInformationQueryResult source)
        {
            StandardInformationViewModel sut = source;

            sut.Should().BeEquivalentTo(source, option =>
            {
                option.WithMapping<StandardInformationViewModel>(r => r.Title, m => m.CourseName);
                option.WithMapping<StandardInformationViewModel>(c => c.ApprovalBody, v => v.RegulatorName);
                option.WithMapping<StandardInformationViewModel>(c => c.Route, v => v.Sector);
                option.Excluding(r => r.StandardUId);
                option.Excluding(r => r.IsRegulatedForProvider);
                option.Excluding(c => c.Duration);
                option.Excluding(c => c.DurationUnits);
                option.Excluding(c => c.CourseType);
                return option;
            });
        }
    }
}
