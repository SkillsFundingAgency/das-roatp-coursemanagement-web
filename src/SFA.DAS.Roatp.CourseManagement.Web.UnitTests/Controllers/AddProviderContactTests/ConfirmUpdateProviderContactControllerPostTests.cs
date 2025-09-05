using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderContactTests;

[TestFixture]
public class ConfirmUpdateProviderContactControllerPostTests
{
    [Test, MoqAutoData]
    public async Task Get_ModelMissingFromSession_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmUpdateProviderContactController sut,
        int ukprn)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns((ProviderContactSessionModel)null);

        var result = await sut.ConfirmCheckContact(ukprn);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public async Task Post_AddProviderContact_RedirectsToContactSavedPage(
      [Frozen] Mock<ISessionService> sessionServiceMock,
      [Frozen] Mock<IMediator> mediatorMock,
      [Greedy] ConfirmUpdateProviderContactController sut,
      int ukprn)
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

        var sessionModel = new ProviderContactSessionModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber
        };

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        var result = await sut.ConfirmCheckContact(ukprn);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContactSaved);
        mediatorMock.Verify(m => m.Send(It.Is<AddProviderContactCommand>(c => c.Ukprn == ukprn && c.UserId == TestConstants.DefaultUserId && c.EmailAddress == email && c.PhoneNumber == phoneNumber), It.IsAny<CancellationToken>()));
    }
}