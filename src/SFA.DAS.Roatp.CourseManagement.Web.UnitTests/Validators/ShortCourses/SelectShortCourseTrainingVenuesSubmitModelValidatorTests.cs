using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ShortCourses;
public class SelectShortCourseTrainingVenuesSubmitModelValidatorTests
{
    [Test]
    public void WhenNoneSelected_IsInvalid()
    {
        var sut = new SelectShortCourseTrainingVenuesSubmitModelValidator();

        var model = new SelectShortCourseTrainingVenuesSubmitModel();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.SelectedProviderLocationIds).WithErrorMessage(SelectShortCourseTrainingVenuesSubmitModelValidator.NoneSelectedErrorMessage);
    }

    [Test]
    public void WhenSelected_IsValid()
    {
        var sut = new SelectShortCourseTrainingVenuesSubmitModelValidator();

        var model = new SelectShortCourseTrainingVenuesSubmitModel()
        {
            SelectedProviderLocationIds = new List<Guid>()
        { Guid.NewGuid() }
        };

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(c => c.SelectedProviderLocationIds);
    }
}
