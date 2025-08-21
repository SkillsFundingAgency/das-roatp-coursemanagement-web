using AutoFixture.NUnit3;
using FluentAssertions;
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
public class ConfirmUpdateStandardsControllerPostTests
{
    [Test, MoqAutoData]
    public void Post_ModelStateIsInvalid_ReturnsViewResult(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmUpdateStandardsController sut,
        int ukprn)
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

        var submitViewModel = new ProviderContactUpdateStandardsSubmitViewModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber
        };

        sut.AddDefaultContextWithUser();
        sut.ModelState.AddModelError("key", "message");

        var result = sut.PostUpdateStandardsEmailAndPhone(ukprn, submitViewModel);

        var viewResult = result as ViewResult;
        var model = viewResult!.Model as ProviderContactUpdateStandardsViewModel;
        model!.BackUrl.Should().BeNull();
        model.EmailAddress.Should().Be(email);
        model.PhoneNumber.Should().Be(phoneNumber);

        sessionServiceMock.Verify(s => s.Set(It.IsAny<ProviderContactSessionModel>()), Times.Never());
    }

    [Test, MoqAutoData]
    public void Post_UpdateExistingStandards_RedirectsToPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmUpdateStandardsController sut,
        int ukprn)
    {
        var email = "test@test.com";
        var phoneNumber = "123445";
        bool updateExistingStandards = true;

        var submitViewModel = new ProviderContactUpdateStandardsSubmitViewModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber,
            HasOptedToUpdateExistingStandards = updateExistingStandards
        };

        sut.AddDefaultContextWithUser();

        var result = sut.PostUpdateStandardsEmailAndPhone(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Set(It.Is<ProviderContactSessionModel>(v => v.UpdateExistingStandards == updateExistingStandards)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContactSelectStandardsForUpdate);
    }

    [Test, MoqAutoData]
    public void Post_UpdateExistingStandardsFalse_RedirectsToPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmUpdateStandardsController sut,
        int ukprn)
    {
        var email = "test@test.com";
        var phoneNumber = "123445";
        bool updateExistingStandards = false;

        var submitViewModel = new ProviderContactUpdateStandardsSubmitViewModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber,
            HasOptedToUpdateExistingStandards = updateExistingStandards
        };

        sut.AddDefaultContextWithUser();

        var result = sut.PostUpdateStandardsEmailAndPhone(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Set(It.Is<ProviderContactSessionModel>(v => v.UpdateExistingStandards == updateExistingStandards)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContactConfirmUpdateStandards);
    }
}