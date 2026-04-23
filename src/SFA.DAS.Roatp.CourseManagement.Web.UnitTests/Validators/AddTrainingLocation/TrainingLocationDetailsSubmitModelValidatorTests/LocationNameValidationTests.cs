using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddTrainingLocation.TrainingLocationDetailsSubmitModelValidatorTests;

[TestFixture]
public class LocationNameValidationTests
{
    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void IsRequired(string locationName)
    {
        var model = new ProviderLocationDetailsSubmitModel { LocationName = locationName };
        var sut = new ProviderLocationDetailsSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.LocationName).WithErrorMessage(ProviderLocationDetailsSubmitModelValidator.LocationNameMissingMessage);
    }

    [Test]
    public void ShouldNotExceedCharacterLimitOf50()
    {
        var model = new ProviderLocationDetailsSubmitModel { LocationName = new string('a', 51) };
        var sut = new ProviderLocationDetailsSubmitModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.LocationName).WithErrorMessage(ProviderLocationDetailsSubmitModelValidator.LocationNameCharacterLimitMessage);
    }

    [TestCase("<")]
    [TestCase(">")]
    [TestCase("^")]
    [TestCase("!")]
    [TestCase("\"")]
    [TestCase("£")]
    [TestCase("$")]
    [TestCase("%")]
    [TestCase("&")]
    [TestCase("*")]
    [TestCase("=")]
    [TestCase("?")]
    [TestCase(";")]
    public async Task ShouldNotAcceptExcludedCharacters(string venueName)
    {
        var model = new ProviderLocationDetailsSubmitModel { LocationName = venueName };
        var sut = new ProviderLocationDetailsSubmitModelValidator();

        var result = await sut.TestValidateAsync(model);

        result
            .ShouldHaveValidationErrorFor(m => m.LocationName)
            .WithErrorMessage(ProviderLocationDetailsSubmitModelValidator.HasExcludedCharactersInVenueNameMessage);
    }
}
