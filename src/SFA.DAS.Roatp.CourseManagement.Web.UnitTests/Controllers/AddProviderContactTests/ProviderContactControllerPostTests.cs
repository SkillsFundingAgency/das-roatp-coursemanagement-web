using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderContactTests;

[TestFixture]
public class ProviderContactControllerPostTests
{
    [Test, MoqAutoData]
    public async Task Post_NullStandards_RedirectsToExpectedPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        GetAllProviderStandardsQueryResult standardsResult,
        int ukprn)
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

        var submitViewModel = new AddProviderContactSubmitViewModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber
        };

        sut.AddDefaultContextWithUser();

        standardsResult.Standards = new List<Standard>();

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderStandardsQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(standardsResult);

        var result = await sut.PostProviderContact(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Set(It.Is<ProviderContactSessionModel>(v => v.EmailAddress == email && v.PhoneNumber == phoneNumber)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContact);
    }

    [Test, MoqAutoData]
    public async Task Post_NoStandards_RedirectsToExpectedPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        int ukprn)
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

        var submitViewModel = new AddProviderContactSubmitViewModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber
        };

        sessionServiceMock.Setup(x => x.Get<ProviderContactSessionModel>()).Returns(new ProviderContactSessionModel { Standards = new List<ProviderContactStandardModel>() });

        sut.AddDefaultContextWithUser();

        var result = await sut.PostProviderContact(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Set(It.Is<ProviderContactSessionModel>(v => v.EmailAddress == email && v.PhoneNumber == phoneNumber)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContact);
    }

    [Test, MoqAutoData]
    public async Task Post_NoEmail_WithStandards_RedirectsToExpectedPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        GetAllProviderStandardsQueryResult standardsResult,
        int ukprn)
    {
        var phoneNumber = "123445";

        var submitViewModel = new AddProviderContactSubmitViewModel
        {
            PhoneNumber = phoneNumber
        };

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderStandardsQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(standardsResult);

        var result = await sut.PostProviderContact(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Set(It.Is<ProviderContactSessionModel>(v => v.PhoneNumber == phoneNumber)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContactConfirmUpdateStandards);
    }

    [Test, MoqAutoData]
    public async Task Post_NoPhoneNumber_WithStandards_RedirectsToExpectedPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        GetAllProviderStandardsQueryResult standardsResult,
        int ukprn)
    {
        var email = "test@test.com";

        var submitViewModel = new AddProviderContactSubmitViewModel
        {
            EmailAddress = email
        };

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderStandardsQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(standardsResult);

        var result = await sut.PostProviderContact(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Set(It.Is<ProviderContactSessionModel>(v => v.EmailAddress == email)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContactConfirmUpdateStandards);
    }

    [Test, MoqAutoData]
    public async Task Post_EmailAndPhoneNumberAndStandards_RedirectsToExpectedPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        GetAllProviderStandardsQueryResult standardsResult,
        int ukprn)
    {
        var email = "test@test.com";
        var phone = "123";

        var submitViewModel = new AddProviderContactSubmitViewModel
        {
            EmailAddress = email,
            PhoneNumber = phone
        };

        sessionServiceMock.Setup(x => x.Get<ProviderContactSessionModel>()).Returns(new ProviderContactSessionModel());

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderStandardsQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(standardsResult);

        var result = await sut.PostProviderContact(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ProviderContactSessionModel>(v => v.EmailAddress == email && v.PhoneNumber == phone)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContactConfirmUpdateStandards);
    }

    [Test, MoqAutoData]
    public async Task Post_EmailAndPhoneNumberAndStandards_NoSession_RedirectsToExpectedPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        GetAllProviderStandardsQueryResult standardsResult,
        int ukprn)
    {
        var email = "test@test.com";
        var phone = "123";

        var submitViewModel = new AddProviderContactSubmitViewModel
        {
            EmailAddress = email,
            PhoneNumber = phone
        };

        sessionServiceMock.Setup(x => x.Get<ProviderContactSessionModel>()).Returns((ProviderContactSessionModel)null);

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderStandardsQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(standardsResult);

        var result = await sut.PostProviderContact(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ProviderContactSessionModel>(v => v.EmailAddress == email && v.PhoneNumber == phone)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContactConfirmUpdateStandards);
    }

    [Test, MoqAutoData]
    public async Task Post_ModelStateIsInvalid_ReturnsViewResult(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        int ukprn)
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

        var submitViewModel = new AddProviderContactSubmitViewModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber
        };

        sut.AddDefaultContextWithUser();
        sut.ModelState.AddModelError("key", "message");

        var result = await sut.PostProviderContact(ukprn, submitViewModel);

        var viewResult = result as ViewResult;
        var model = viewResult!.Model as AddProviderContactViewModel;
        model!.EmailAddress.Should().Be(email);
        model.PhoneNumber.Should().Be(phoneNumber);

        sessionServiceMock.Verify(s => s.Set(It.IsAny<ProviderContactSessionModel>()), Times.Never());
    }
}
