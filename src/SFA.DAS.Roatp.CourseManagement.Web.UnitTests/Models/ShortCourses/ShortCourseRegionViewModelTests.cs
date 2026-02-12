using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses;
public class ShortCourseRegionViewModelTests
{
    [Test, AutoData]
    public void RegionModelOperator_TransformsFromRegionModel(RegionModel source)
    {
        ShortCourseRegionViewModel sut = source;

        Assert.That(sut.Id, Is.EqualTo(source.Id));
        Assert.That(sut.SubregionName, Is.EqualTo(source.SubregionName));
        Assert.That(sut.RegionName, Is.EqualTo(source.RegionName));
    }
}
