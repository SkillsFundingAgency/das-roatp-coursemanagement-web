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
public class ProviderContactUpdateStandardsControllerGetTests
{
    [Test, MoqAutoData]
    public void Get_ModelMissingFromSession_RedirectsToAddProviderContact(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ProviderContactUpdateStandardsController sut,
        int ukprn)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns((ProviderContactSessionModel)null);

        var result = sut.UpdateStandardsEmailAndPhone(ukprn);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContact);
    }

    [Test, MoqAutoData]
    public void Get_ModelInSession_PopulatesExpectedModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ProviderContactUpdateStandardsController sut,
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

        var result = sut.UpdateStandardsEmailAndPhone(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as ProviderContactUpdateStandardsViewModel;
        model!.BackUrl.Should().BeNull();
        model.EmailAddress.Should().Be(email);
        model.PhoneNumber.Should().Be(phoneNumber);
        model.UpdateExistingStandards.Should().Be(updateExistingStandards);
        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
    }
}