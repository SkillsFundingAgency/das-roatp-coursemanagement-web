using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses;
public class ShortCourseContactDetailsViewModelTests
{
    [Test, MoqAutoData]
    public void EditShortCourseContactDetailsViewModel_ImplicitOperator_MapsPropertiesCorrectly(
        GetStandardDetailsQueryResult source)
    {
        // Act
        ShortCourseContactDetailsViewModel sut = source;

        // Assert
        Assert.That(sut.ContactUsEmail, Is.EqualTo(source.ContactUsEmail));
        Assert.That(sut.StandardInfoUrl, Is.EqualTo(source.StandardInfoUrl));
        Assert.That(sut.ContactUsPhoneNumber, Is.EqualTo(source.ContactUsPhoneNumber));
    }
}
