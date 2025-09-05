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

    [Test, MoqAutoData]
    public void Get_ModelInSession_EmailandPhone_PopulatesExpectedModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmUpdateStandardsController sut,
        int ukprn,
        bool? updateExistingStandards
        )
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

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

        viewResult.ViewName.Should().Contain("UpdateStandardsPhoneAndEmail");
        model!.BackUrl.Should().BeNull();
        model.EmailAddress.Should().Be(email);
        model.PhoneNumber.Should().Be(phoneNumber);
        model.HasOptedToUpdateExistingStandards.Should().Be(updateExistingStandards);
        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void Get_ModelInSession_EmailOnly_PopulatesExpectedModel(
       [Frozen] Mock<ISessionService> sessionServiceMock,
       [Greedy] ConfirmUpdateStandardsController sut,
       int ukprn,
       bool? updateExistingStandards
       )
    {
        var email = "test@test.com";

        var sessionModel = new ProviderContactSessionModel
        {
            EmailAddress = email,
            UpdateExistingStandards = updateExistingStandards
        };

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.ConfirmUpdateStandards(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as ConfirmUpdateStandardsViewModel;
        viewResult.ViewName.Should().Contain("UpdateStandardsEmailOnly");
        model!.BackUrl.Should().BeNull();
        model.EmailAddress.Should().Be(email);
        model.PhoneNumber.Should().BeNull();
        model.HasOptedToUpdateExistingStandards.Should().Be(updateExistingStandards);
        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void Get_ModelInSession_PhoneOnly_PopulatesExpectedModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmUpdateStandardsController sut,
        int ukprn,
        bool? updateExistingStandards
        )
    {
        var phoneNumber = "123445";

        var sessionModel = new ProviderContactSessionModel
        {
            PhoneNumber = phoneNumber,
            UpdateExistingStandards = updateExistingStandards
        };

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.ConfirmUpdateStandards(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as ConfirmUpdateStandardsViewModel;
        viewResult.ViewName.Should().Contain("UpdateStandardsPhoneOnly");
        model!.BackUrl.Should().BeNull();
        model.EmailAddress.Should().BeNull();
        model.PhoneNumber.Should().Be(phoneNumber);
        model.HasOptedToUpdateExistingStandards.Should().Be(updateExistingStandards);
        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
    }
}