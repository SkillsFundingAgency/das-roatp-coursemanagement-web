using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ShortCourses;
public class SelectShortCourseLocationOptionsSubmitModelValidatorTests
{
    [Test]
    public void WhenNoneSelected_IsInvalid()
    {
        var sut = new SelectShortCourseLocationOptionsSubmitModelValidator();

        var model = new SelectShortCourseLocationOptionsSubmitModel();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.SelectedLocationOptions).WithErrorMessage(SelectShortCourseLocationOptionsSubmitModelValidator.NoneSelectedErrorMessage);
    }

    [Test]
    public void WhenSelected_IsValid()
    {
        var sut = new SelectShortCourseLocationOptionsSubmitModelValidator();

        var model = new SelectShortCourseLocationOptionsSubmitModel() { SelectedLocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.ProviderLocation } };

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(c => c.SelectedLocationOptions);
    }
}
