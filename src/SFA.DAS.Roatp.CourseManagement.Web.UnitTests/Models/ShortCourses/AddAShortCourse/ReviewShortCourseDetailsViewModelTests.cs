using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using System.Linq;

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
        Assert.That(sut.ContactUsEmail, Is.EqualTo(sessionModel.ContactInformation.ContactUsEmail));
        Assert.That(sut.ContactUsPhoneNumber, Is.EqualTo(sessionModel.ContactInformation.ContactUsPhoneNumber));
        Assert.That(sut.StandardInfoUrl, Is.EqualTo(sessionModel.ContactInformation.StandardInfoUrl));
        Assert.That(sut.LocationOptions, Is.EqualTo(sessionModel.LocationOptions));
        Assert.That(sut.DeliveryLocations.First, Is.EqualTo(expectedLocationOptionText));
        Assert.That(sut.TrainingVenues, Is.EqualTo(sessionModel.TrainingVenues.Select(x => x.LocationName).ToList()));
        Assert.That(sut.HasNationalDeliveryOption, Is.EqualTo(expectedHasNationalDeliveryOptionText));
        Assert.That(sut.TrainingRegions, Is.EqualTo(sessionModel.TrainingRegions.Select(x => x.SubregionName).ToList()));
        Assert.That(sut.DeliversAtEmployerLocation, Is.EqualTo(sessionModel.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation)));
    }
}
