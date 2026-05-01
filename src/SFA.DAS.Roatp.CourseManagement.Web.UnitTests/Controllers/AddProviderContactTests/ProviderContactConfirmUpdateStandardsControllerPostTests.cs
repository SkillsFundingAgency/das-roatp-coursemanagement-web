using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderContactTests;

[TestFixture]
public class ProviderContactConfirmUpdateStandardsControllerPostTests
{
    [Test]
    [MoqInlineAutoData("test@test.com", "123445", true, false, false)]
    [MoqInlineAutoData("test@test.com", "", false, true, false)]
    [MoqInlineAutoData("", "123445", false, false, true)]
    [MoqInlineAutoData("test@test.com", null, false, true, false)]
    [MoqInlineAutoData(null, "123445", false, false, true)]
    public void Post_ModelStateIsInvalid_VariationOfEmailAndPhoneNumberInputs_SetsModelFlagCorrectlyAndReturnsViewResult(
        string email,
        string phoneNumber,
        bool expectedEmailAddressAndPhoneNumberUpdate,
        bool expectedEmailAddressOnlyUpdate,
        bool expectedPhoneNumberOnlyUpdate,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmUpdateStandardsController sut,
        int ukprn)
    {
        var submitViewModel = new ConfirmUpdateStandardsSubmitViewModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber
        };

        sut.AddDefaultContextWithUser();
        sut.ModelState.AddModelError("key", "message");

        var result = sut.PostConfirmUpdateStandards(ukprn, submitViewModel);

        var viewResult = result as ViewResult;
        var model = viewResult!.Model as ConfirmUpdateStandardsViewModel;
        viewResult.ViewName.Should().Contain("UpdateStandardsContactDetails");
        model.EmailAddress.Should().Be(email);
        model.PhoneNumber.Should().Be(phoneNumber);
        model.EmailAddressAndPhoneNumberUpdate.Should().Be(expectedEmailAddressAndPhoneNumberUpdate);
        model.EmailAddressOnlyUpdate.Should().Be(expectedEmailAddressOnlyUpdate);
        model.PhoneNumberOnlyUpdate.Should().Be(expectedPhoneNumberOnlyUpdate);

        sessionServiceMock.Verify(s => s.Set(It.IsAny<ProviderContactSessionModel>()), Times.Never());
    }

    [Test, MoqAutoData]
    public void Post_UpdateExistingStandards_RedirectsToPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<ConfirmUpdateStandardsSubmitViewModel>> validator,
        [Greedy] ConfirmUpdateStandardsController sut,
        int ukprn)
    {
        var email = "test@test.com";
        var phoneNumber = "123445";
        bool updateExistingStandards = true;

        var submitViewModel = new ConfirmUpdateStandardsSubmitViewModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber,
            HasOptedToUpdateExistingStandards = updateExistingStandards
        };

        validator.Setup(x => x.Validate(It.IsAny<ConfirmUpdateStandardsSubmitViewModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        var result = sut.PostConfirmUpdateStandards(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Set(It.Is<ProviderContactSessionModel>(v => v.UpdateExistingStandards == updateExistingStandards)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContactSelectStandardsForUpdate);
    }

    [Test, MoqAutoData]
    public void Post_UpdateExistingStandardsFalse_RedirectsToPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<ConfirmUpdateStandardsSubmitViewModel>> validator,
        [Greedy] ConfirmUpdateStandardsController sut,
        int ukprn)
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

        var submitViewModel = new ConfirmUpdateStandardsSubmitViewModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber,
            HasOptedToUpdateExistingStandards = false
        };

        validator.Setup(x => x.Validate(It.IsAny<ConfirmUpdateStandardsSubmitViewModel>())).Returns(new ValidationResult());

        sut.AddDefaultContextWithUser();

        var result = sut.PostConfirmUpdateStandards(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Set(It.Is<ProviderContactSessionModel>(v => v.UpdateExistingStandards == false)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContact);
    }
}