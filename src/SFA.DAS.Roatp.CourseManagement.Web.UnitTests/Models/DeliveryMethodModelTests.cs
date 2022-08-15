using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models
{
    [TestFixture]
    public class DeliveryMethodModelTests
    {
        [TestCase(true, false, DeliveryMethodModel.DayReleaseDescription)]
        [TestCase(true, false, DeliveryMethodModel.DayReleaseDescription)]
        [TestCase(true, true, DeliveryMethodModel.DayAndBlockReleaseDescription)]
        [TestCase(false, false, "")]
        public void ToSummary_ReturnsAppropriateSummary(bool hasDayRelease, bool hasBlockRelease, string expectedSummary)
        {
            var sut = new DeliveryMethodModel()
            {
                HasDayReleaseDeliveryOption = hasDayRelease,
                HasBlockReleaseDeliveryOption = hasBlockRelease
            };

            var actualSummary = sut.ToSummary();

            Assert.AreEqual(expectedSummary, actualSummary);
        }
    }
}
