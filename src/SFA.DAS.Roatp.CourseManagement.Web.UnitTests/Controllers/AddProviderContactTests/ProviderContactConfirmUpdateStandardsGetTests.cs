using AutoFixture.NUnit3;
using FluentAssertions;
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
public class ProviderContactConfirmUpdateStandardsGetTests
{
    [Test, MoqAutoData]
    public void Get_ModelMissingFromSession_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmUpdateStandardsController sut,
        int ukprn)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns((ProviderContactSessionModel)null);

        var result = sut.ConfirmUpdateStandards(ukprn);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test]
    [MoqInlineAutoData("test@test.com", "123445", true, false, false)]
    [MoqInlineAutoData("test@test.com", "", false, true, false)]
    [MoqInlineAutoData("", "123445", false, false, true)]
    [MoqInlineAutoData("test@test.com", null, false, true, false)]
    [MoqInlineAutoData(null, "123445", false, false, true)]
    public void Get_ModelInSession_VariationOfEmailAndPhoneNumberInputs_PopulatesExpectedModel(
        string email,
        string phoneNumber,
        bool expectedEmailAddressAndPhoneNumberUpdate,
        bool expectedEmailAddressOnlyUpdate,
        bool expectedPhoneNumberOnlyUpdate,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmUpdateStandardsController sut,
        int ukprn,
        bool? updateExistingStandards
        )
    {
        var sessionModel = new ProviderContactSessionModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber,
            UpdateExistingStandards = updateExistingStandards
        };

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.ConfirmUpdateStandards(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as ConfirmUpdateStandardsViewModel;

        viewResult.ViewName.Should().Contain("UpdateStandardsContactDetails");
        model.EmailAddress.Should().Be(email);
        model.PhoneNumber.Should().Be(phoneNumber);
        model.HasOptedToUpdateExistingStandards.Should().Be(updateExistingStandards);
        model.EmailAddressAndPhoneNumberUpdate.Should().Be(expectedEmailAddressAndPhoneNumberUpdate);
        model.EmailAddressOnlyUpdate.Should().Be(expectedEmailAddressOnlyUpdate);
        model.PhoneNumberOnlyUpdate.Should().Be(expectedPhoneNumberOnlyUpdate);
        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
    }
}