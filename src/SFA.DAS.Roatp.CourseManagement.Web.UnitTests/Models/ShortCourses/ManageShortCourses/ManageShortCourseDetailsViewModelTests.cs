using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.ManageShortCourses;
public class ManageShortCourseDetailsViewModelTests
{
    [Test, MoqAutoData]
    public void ManageShortCourseDetailsViewModel_ImplicitOperator_MapsPropertiesCorrectly(
        GetProviderCourseDetailsQueryResult source)
    {
        // Act
        ManageShortCourseDetailsViewModel sut = source;

        // Assert
        Assert.That(sut.ShortCourseInformation.LarsCode, Is.EqualTo(source.LarsCode));
        Assert.That(sut.ShortCourseInformation.CourseName, Is.EqualTo(source.CourseName));
        Assert.That(sut.ShortCourseInformation.Level, Is.EqualTo(source.Level));
        Assert.That(sut.ShortCourseInformation.IfateReferenceNumber, Is.EqualTo(source.IFateReferenceNumber));
        Assert.That(sut.ShortCourseInformation.Sector, Is.EqualTo(source.Sector));
        Assert.That(sut.ShortCourseInformation.RegulatorName, Is.EqualTo(source.RegulatorName));
        Assert.That(sut.ShortCourseInformation.ApprenticeshipType, Is.EqualTo(source.ApprenticeshipType));
        Assert.That(sut.ShortCourseInformation.IsRegulatedForProvider, Is.EqualTo(source.IsRegulatedForProvider));
        Assert.That(sut.ShortCourseInformation.Duration, Is.EqualTo(source.Duration));
        Assert.That(sut.ShortCourseInformation.DurationUnits, Is.EqualTo(source.DurationUnits));
        Assert.That(sut.ShortCourseInformation.CourseType, Is.EqualTo(source.CourseType));
        Assert.That(sut.ContactInformation.ContactUsEmail, Is.EqualTo(source.ContactUsEmail));
        Assert.That(sut.ContactInformation.ContactUsPhoneNumber, Is.EqualTo(source.ContactUsPhoneNumber));
        Assert.That(sut.ContactInformation.StandardInfoUrl, Is.EqualTo(source.StandardInfoUrl));
        Assert.That(sut.LocationInformation.TrainingVenues, Is.EqualTo(source.ProviderCourseLocations.Where(x => x.LocationType == LocationType.Provider).Select(x => x.LocationName).ToList()));
        Assert.That(sut.LocationInformation.TrainingRegions, Is.EqualTo(source.ProviderCourseLocations.Where(x => x.LocationType == LocationType.Regional).Select(x => x.SubregionName).ToList()));
    }


    [Test]
    [InlineAutoData(LocationType.Provider, false, ShortCourseLocationOption.ProviderLocation, "At your training venue", "No")]
    [InlineAutoData(LocationType.National, false, ShortCourseLocationOption.EmployerLocation, "At employer’s location", "Yes")]
    [InlineAutoData(LocationType.Regional, false, ShortCourseLocationOption.EmployerLocation, "At employer’s location", "No")]
    [InlineAutoData(LocationType.Provider, true, ShortCourseLocationOption.Online, "Online", "No")]
    public void ManageShortCourseDetailsViewModel_ImplicitOperator_MapsLocationOptionsCorrectly(
        LocationType locationType,
        bool hasOnlineDeliveryOption,
        ShortCourseLocationOption expectedLocationOption,
        string expectedLocationOptionText,
        string expectedHasNationalDeliveryOptionText,
        GetProviderCourseDetailsQueryResult source)
    {
        // Arrange
        source.ProviderCourseLocations = new List<ProviderCourseLocation>()
        {
            new ProviderCourseLocation
            {
                LocationName = "Test",
                LocationType = locationType
            }
        };

        source.HasOnlineDeliveryOption = hasOnlineDeliveryOption;

        if (hasOnlineDeliveryOption)
        {
            source.ProviderCourseLocations = new List<ProviderCourseLocation>();
        }


        var LocationOptions = new List<ShortCourseLocationOption>();
        LocationOptions.Add(expectedLocationOption);


        // Act
        ManageShortCourseDetailsViewModel sut = source;

        // Assert
        Assert.That(sut.LocationInformation.LocationOptions, Is.EqualTo(LocationOptions));
        Assert.That(sut.LocationInformation.DeliveryLocations.First, Is.EqualTo(expectedLocationOptionText));
        Assert.That(sut.LocationInformation.HasNationalDeliveryOption, Is.EqualTo(expectedHasNationalDeliveryOptionText));
        Assert.That(sut.LocationInformation.DeliversAtEmployerLocation, Is.EqualTo(LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation)));
    }
}
