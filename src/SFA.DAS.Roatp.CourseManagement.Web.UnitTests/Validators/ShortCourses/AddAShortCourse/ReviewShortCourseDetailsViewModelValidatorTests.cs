using AutoFixture.NUnit3;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.AddAShortCourse;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ShortCourses.AddAShortCourse;
public class ReviewShortCourseDetailsViewModelValidatorTests
{
    [Test]
    [InlineAutoData("", "0123456790", "www.test.com")]
    [InlineAutoData("test@test.com", "", "www.test.com")]
    [InlineAutoData("test@test.com", "0123456790", "")]
    [InlineAutoData(null, "0123456790", "www.test.com")]
    [InlineAutoData("test@test.com", null, "www.test.com")]
    [InlineAutoData("test@test.com", "0123456790", null)]
    public void ContactDetailsMissing_ShowsExpectedErrorMessage(
        string email,
        string phoneNumber,
        string website,
        ShortCourseSessionModel sessionModel)
    {
        sessionModel.ContactInformation = new ContactInformationModel()
        {
            ContactUsEmail = email,
            ContactUsPhoneNumber = phoneNumber,
            StandardInfoUrl = website,
        };

        ReviewShortCourseDetailsViewModel model = sessionModel;

        model.ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var sut = new ReviewShortCourseDetailsViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(s => s)
            .WithErrorMessage($"Enter all contact details for this {model.ApprenticeshipTypeLower}");
    }

    [Test, AutoData]
    public void TrainingOptionsMissing_ShowsExpectedErrorMessage(
        ShortCourseSessionModel sessionModel)
    {
        sessionModel.LocationOptions = [];

        ReviewShortCourseDetailsViewModel model = sessionModel;

        model.ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var sut = new ReviewShortCourseDetailsViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(s => s.DeliveryLocations)
            .WithErrorMessage($"Select training options for this {model.ApprenticeshipTypeLower}");
    }

    [Test, AutoData]
    public void TrainingVenuesMissing_WhenProviderLocation_ShowsExpectedErrorMessage(
        ShortCourseSessionModel sessionModel)
    {
        sessionModel.LocationOptions = [ShortCourseLocationOption.ProviderLocation];

        sessionModel.TrainingVenues = new List<TrainingVenueModel>();

        ReviewShortCourseDetailsViewModel model = sessionModel;

        model.ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var sut = new ReviewShortCourseDetailsViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(s => s.TrainingVenues)
            .WithErrorMessage(ReviewShortCourseDetailsViewModelValidator.IncompleteErrorMessage);
    }

    [Test, AutoData]
    public void HasNationalDeliveryOptionIsNull_WhenEmployerLocation_ShowsExpectedErrorMessage(
        ShortCourseSessionModel sessionModel)
    {
        sessionModel.LocationOptions = [ShortCourseLocationOption.EmployerLocation];

        sessionModel.HasNationalDeliveryOption = null;

        ReviewShortCourseDetailsViewModel model = sessionModel;

        model.ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var sut = new ReviewShortCourseDetailsViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(s => s.HasNationalDeliveryOption)
            .WithErrorMessage(ReviewShortCourseDetailsViewModelValidator.IncompleteErrorMessage);
    }

    [Test, AutoData]
    public void TrainingRegionsMissing_WhenEmployerLocationAndHasNationalDeliveryOptionIsFalse_ShowsExpectedErrorMessage(
        ShortCourseSessionModel sessionModel)
    {
        sessionModel.LocationOptions = [ShortCourseLocationOption.EmployerLocation];

        sessionModel.HasNationalDeliveryOption = false;

        sessionModel.TrainingRegions = new List<TrainingRegionModel>();

        ReviewShortCourseDetailsViewModel model = sessionModel;

        model.ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var sut = new ReviewShortCourseDetailsViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(s => s.TrainingRegions)
            .WithErrorMessage(ReviewShortCourseDetailsViewModelValidator.IncompleteErrorMessage);
    }

    [Test, AutoData]
    public void SessionDataIsValid_NoErrorMessage(
    ShortCourseSessionModel sessionModel)
    {
        ReviewShortCourseDetailsViewModel model = sessionModel;

        model.ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var sut = new ReviewShortCourseDetailsViewModelValidator();

        var result = sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
