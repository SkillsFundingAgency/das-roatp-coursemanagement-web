using System.Linq;
using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.AddAShortCourse;
public class ReviewShortCourseDetailsViewModelTests
{
    [Test]
    [InlineAutoData(ShortCourseLocationOption.ProviderLocation, true, "At your training venue", "Yes")]
    [InlineAutoData(ShortCourseLocationOption.EmployerLocation, false, "At employer’s location", "No")]
    [InlineAutoData(ShortCourseLocationOption.Online, null, "Online", null)]
    [InlineAutoData((ShortCourseLocationOption)10, null, "10", null)]
    public void ReviewShortCourseDetailsViewModel_ImplicitOperator_MapsPropertiesCorrectly(
        ShortCourseLocationOption locationOption,
        bool? hasNationalDeliveryOption,
        string expectedLocationOptionText,
        string expectedHasNationalDeliveryOptionText,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sessionModel.LocationOptions =
        [
            locationOption
        ];

        sessionModel.HasNationalDeliveryOption = hasNationalDeliveryOption;

        // Act
        ReviewShortCourseDetailsViewModel sut = sessionModel;

        // Assert
        Assert.That(sut.ShortCourseInformation, Is.EqualTo(sessionModel.ShortCourseInformation));
        Assert.That(sut.ContactInformation.ContactUsEmail, Is.EqualTo(sessionModel.ContactInformation.ContactUsEmail));
        Assert.That(sut.ContactInformation.ContactUsPhoneNumber, Is.EqualTo(sessionModel.ContactInformation.ContactUsPhoneNumber));
        Assert.That(sut.ContactInformation.StandardInfoUrl, Is.EqualTo(sessionModel.ContactInformation.StandardInfoUrl));
        Assert.That(sut.LocationInformation.LocationOptions, Is.EqualTo(sessionModel.LocationOptions));
        Assert.That(sut.LocationInformation.DeliveryLocations.First, Is.EqualTo(expectedLocationOptionText));
        Assert.That(sut.LocationInformation.TrainingVenues, Is.EqualTo(sessionModel.TrainingVenues.Select(x => x.LocationName).ToList()));
        Assert.That(sut.LocationInformation.HasNationalDeliveryOption, Is.EqualTo(expectedHasNationalDeliveryOptionText));
        Assert.That(sut.LocationInformation.TrainingRegions, Is.EqualTo(sessionModel.TrainingRegions.Select(x => x.SubregionName).OrderBy(x => x).ToList()));
        Assert.That(sut.LocationInformation.DeliversAtEmployerLocation, Is.EqualTo(sessionModel.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation)));
    }
}
