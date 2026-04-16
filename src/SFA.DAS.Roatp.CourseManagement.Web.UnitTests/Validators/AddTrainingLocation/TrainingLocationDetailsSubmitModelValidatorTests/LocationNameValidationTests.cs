using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddTrainingLocation.TrainingLocationDetailsSubmitModelValidatorTests;

[TestFixture]
public class LocationNameValidationTests
{
    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public async Task IsRequired(string locationName)
    {
        var model = new ProviderLocationDetailsSubmitModel { LocationName = locationName };
        var sut = new TrainingLocationDetailsSubmitModelValidator();

        var result = await sut.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(m => m.LocationName).WithErrorMessage(TrainingLocationDetailsSubmitModelValidator.VenueNameMissingMessage);
    }

    [Test]
    public async Task ShouldNotExceedCharacterLimitOf50()
    {
        var model = new ProviderLocationDetailsSubmitModel { LocationName = new string('a', 51) };
        var sut = new TrainingLocationDetailsSubmitModelValidator();

        var result = await sut.TestValidateAsync(model)
            ;
        result.ShouldHaveValidationErrorFor(m => m.LocationName).WithErrorMessage(TrainingLocationDetailsSubmitModelValidator.VenueNameCharacterLimitMessage);
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
        var sut = new TrainingLocationDetailsSubmitModelValidator();

        var result = await sut.TestValidateAsync(model);

        result
            .ShouldHaveValidationErrorFor(m => m.LocationName)
            .WithErrorMessage(CommonValidationErrorMessage.HasExcludedCharactersInVenueNameMessage);
    }
}
