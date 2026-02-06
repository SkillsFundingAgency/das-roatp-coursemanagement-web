using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ShortCourses.AddAShortCourse;
public class SelectShortCourseSubmitModelValidatorTests
{
    [TestCase("0", true)]
    [TestCase("12", true)]
    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase(" ", false)]
    public void SelectedLarsCode_Validation(string larsCode, bool isValid)
    {
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var model = new SelectShortCourseSubmitModel() { SelectedLarsCode = larsCode, ApprenticeshipType = apprenticeshipType };
        var sut = new SelectShortCourseSubmitModelValidator();

        var result = sut.TestValidate(model);

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(m => m.SelectedLarsCode);
        else
            result.ShouldHaveValidationErrorFor(m => m.SelectedLarsCode);
    }
}