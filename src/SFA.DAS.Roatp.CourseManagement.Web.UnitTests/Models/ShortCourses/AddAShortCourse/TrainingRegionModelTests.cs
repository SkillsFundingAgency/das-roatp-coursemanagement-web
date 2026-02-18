using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.AddAShortCourse;
public class TrainingRegionModelTests
{
    [Test, AutoData]
    public void Operator_TransformsFromRegionModel(RegionModel source)
    {
        // Arrange
        TrainingRegionModel sut = source;

        // Asset
        Assert.That(sut.SubregionId, Is.EqualTo(source.Id));
        Assert.That(sut.SubregionName, Is.EqualTo(source.SubregionName));
    }
}
