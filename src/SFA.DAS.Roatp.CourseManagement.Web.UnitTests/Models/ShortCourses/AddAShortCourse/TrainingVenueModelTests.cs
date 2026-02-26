using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
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

    [Test, AutoData]
    public void Operator_TransformsToProviderCourseLocationCommandModel(TrainingVenueModel source)
    {
        // Arrange
        ProviderCourseLocationCommandModel model = source;

        // Asset
        Assert.That(model.ProviderLocationId, Is.EqualTo(source.ProviderLocationId));
        Assert.That(model.HasBlockReleaseDeliveryOption, Is.False);
        Assert.That(model.HasDayReleaseDeliveryOption, Is.False);
    }
}
