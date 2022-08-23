using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddAStandard
{
    [TestFixture]
    public class TrainingLocationListViewModelValidatorTests
    {
        [Test]
        public void ValidModel_NoErrors()
        {
            var model = new TrainingLocationListViewModel { FirstLocation = "location 1" };
            var sut = new TrainingLocationListViewModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.FirstLocation);
        }

        [Test]
        public void InvalidModel_LocationError()
        {
            var model = new TrainingLocationListViewModel();
            var sut = new TrainingLocationListViewModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.FirstLocation)
                .WithErrorMessage(TrainingLocationListViewModelValidator.TrainingLocationErrorMessage);
        }
    }
}
