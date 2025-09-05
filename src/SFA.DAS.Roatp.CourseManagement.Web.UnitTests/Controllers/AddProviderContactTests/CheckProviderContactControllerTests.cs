using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderContactTests;

[TestFixture]
public class CheckProviderContactControllerTests
{
    [Test, MoqAutoData]
    public async Task Get_NoExistingRecord_RedirectsToAddProviderContactDetails(
       [Frozen] Mock<ISessionService> sessionServiceMock,
       [Frozen] Mock<IMediator> mediatorMock,
       [Greedy] CheckProviderContactController sut,
       int ukprn)
    {
        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(x => x.Send(It.Is<GetLatestProviderContactQuery>(x => x.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync((GetLatestProviderContactQueryResult)null);

        var result = await sut.CheckProviderContact(ukprn);
        var redirectResult = result as RedirectToRouteResult;

        mediatorMock.Verify(s => s.Send(It.Is<GetLatestProviderContactQuery>(x => x.Ukprn == ukprn), It.IsAny<CancellationToken>()), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddProviderContactDetails);
    }

    [Test, MoqAutoData]
    public async Task Get_SetsSessionModelAndBuildsExpectedViewModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CheckProviderContactController sut,
        GetLatestProviderContactQueryResult queryResult,
        string addProviderContactDetailsLink,
        int ukprn)
    {
        sut.AddDefaultContextWithUser().AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddProviderContactDetails, addProviderContactDetailsLink);

        mediatorMock
            .Setup(x => x.Send(It.Is<GetLatestProviderContactQuery>(x => x.Ukprn == ukprn),
                It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        var result = await sut.CheckProviderContact(ukprn);

        var viewResult = result as ViewResult;

        mediatorMock.Verify(
            s => s.Send(It.Is<GetLatestProviderContactQuery>(x => x.Ukprn == ukprn), It.IsAny<CancellationToken>()),
            Times.Once);
        sessionServiceMock.Verify(
            s => s.Set(It.Is<ProviderContactSessionModel>(s =>
                s.EmailAddress == queryResult.EmailAddress && s.PhoneNumber == queryResult.PhoneNumber)), Times.Once);
        var model = viewResult!.Model as CheckProviderContactViewModel;
        model!.EmailAddress.Should().Be(queryResult.EmailAddress);
        model.PhoneNumber.Should().Be(queryResult.PhoneNumber);
        model.ChangeProviderContactDetailsLink.Should().Be(addProviderContactDetailsLink);
        model.BackUrl.Should().BeNull();
    }
}
