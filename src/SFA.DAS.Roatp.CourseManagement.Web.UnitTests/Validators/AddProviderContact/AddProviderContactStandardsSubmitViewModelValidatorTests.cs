using System.Collections.Generic;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddProviderContact;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddProviderContact;

[TestFixture]
public class AddProviderContactStandardsSubmitViewModelValidatorTests
{
    [Test]
    public void NoSelectionIsMade_ShowsExpectedErrorMessage()
    {
        var model = new AddProviderContactStandardsSubmitViewModel();
        var sut = new AddProviderContactStandardsSubmitViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.SelectedProviderCourseIds)
            .WithErrorMessage(AddProviderContactStandardsSubmitViewModelValidator.SelectAStandardErrorMessage);
    }

    [Test]
    public void SelectionIsMade_ShowsNoErrorMessage()
    {
        var model = new AddProviderContactStandardsSubmitViewModel
        {
            SelectedProviderCourseIds = new List<int> { 1 }
        };
        var sut = new AddProviderContactStandardsSubmitViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
