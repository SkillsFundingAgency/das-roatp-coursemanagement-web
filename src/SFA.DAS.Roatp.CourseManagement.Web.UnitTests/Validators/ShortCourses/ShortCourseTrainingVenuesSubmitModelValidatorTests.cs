using System;
using System.Collections.Generic;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ShortCourses;
public class ShortCourseTrainingVenuesSubmitModelValidatorTests
{
    [Test]
    public void WhenNoneSelected_IsInvalid()
    {
        var sut = new ShortCourseTrainingVenuesSubmitModelValidator();

        var model = new ShortCourseTrainingVenuesSubmitModel();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.SelectedProviderLocationIds).WithErrorMessage(ShortCourseTrainingVenuesSubmitModelValidator.NoneSelectedErrorMessage);
    }

    [Test]
    public void WhenSelected_IsValid()
    {
        var sut = new ShortCourseTrainingVenuesSubmitModelValidator();

        var model = new ShortCourseTrainingVenuesSubmitModel()
        {
            SelectedProviderLocationIds = new List<Guid>()
        { Guid.NewGuid() }
        };

        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(c => c.SelectedProviderLocationIds);
    }
}
