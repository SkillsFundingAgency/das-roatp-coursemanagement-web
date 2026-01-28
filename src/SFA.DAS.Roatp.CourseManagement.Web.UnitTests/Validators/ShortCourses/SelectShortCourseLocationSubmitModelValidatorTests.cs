using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ShortCourses;
public class SelectShortCourseLocationSubmitModelValidatorTests
{
    [Test]
    public void WhenNoneSelected_ReturnsValidationError()
    {
        var sut = new SelectShortCourseLocationSubmitModelValidator();

        var model = new SelectShortCourseLocationSubmitModel();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.ShortCourseLocations).WithErrorMessage(SelectShortCourseLocationSubmitModelValidator.NoneSelectedErrorMessage);
    }

    [Test]
    public void WhenSelected_DoesNotReturnValidationError()
    {
        var sut = new SelectShortCourseLocationSubmitModelValidator();

        var model = new SelectShortCourseLocationSubmitModel() { ShortCourseLocations = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.ProviderLocation } };

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(c => c.ShortCourseLocations);
    }
}
