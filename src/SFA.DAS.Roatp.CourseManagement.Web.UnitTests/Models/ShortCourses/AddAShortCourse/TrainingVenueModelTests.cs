using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.AddAShortCourse;
public class TrainingVenueModelTests
{
    [Test, AutoData]
    public void Operator_TransformsFromProviderLocation(ProviderLocation source)
    {
        // Arrange
        TrainingVenueModel sut = source;

        // Asset
        Assert.That(sut.LocationType, Is.EqualTo(source.LocationType));
        Assert.That(sut.LocationName, Is.EqualTo(source.LocationName));
        Assert.That(sut.ProviderLocationId, Is.EqualTo(source.NavigationId));
    }
}
