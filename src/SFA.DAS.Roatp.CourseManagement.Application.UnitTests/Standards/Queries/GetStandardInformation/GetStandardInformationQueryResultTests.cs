using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Standards.Queries.GetStandardInformation
{
    [TestFixture]
    public class GetStandardInformationQueryResultTests
    {
        [TestCase("Approval body name", true)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void IsRegulatedStandard_ReturnsCorrectValue(string approvalBodyName, bool isValid)
        {
            var sut = new GetStandardInformationQueryResult()
            {
                ApprovalBody = approvalBodyName
            };

            sut.IsRegulatedStandard.Should().Be(isValid);
        }
    }
}
