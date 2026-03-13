using System;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Forecasts;
using static SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers.CommonHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Forecasts;

public class GetDateRangeTests
{
    [Test]
    public void GetDateRange_Returns_Correctly_Formatted_String()
    {
        // Arrange
        DateTime startDate = GetDate(2024, 1, 1);
        DateTime endDate = GetDate(2024, 3, 31);
        // Act
        string result = ForecastModel.GetDateRange(startDate, endDate);
        // Assert
        Assert.AreEqual("Jan 1st to Mar 31st 2024", result);
    }
}
