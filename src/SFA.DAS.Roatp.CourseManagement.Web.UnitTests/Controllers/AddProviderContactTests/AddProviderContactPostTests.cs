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
public class AddProviderContactPostTests
{
    [Test, MoqAutoData]
    public void Post_NoErrors_RedirectsToNextPage(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddProviderContactController sut,
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

        var result = sut.PostProviderContact(ukprn, submitViewModel);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Set(It.Is<ProviderContactSessionModel>(v => v.EmailAddress == email && v.PhoneNumber == phoneNumber)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.GetAddProviderContact);

    }

    [Test, MoqAutoData]
    public void Post_ModelStateIsInvalid_ReturnsViewResult(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddProviderContactController sut,
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

        var result = sut.PostProviderContact(ukprn, submitViewModel);

        var viewResult = result as ViewResult;
        var model = viewResult!.Model as AddProviderContactViewModel;
        model!.BackUrl.Should().BeNull();
        model.EmailAddress.Should().Be(email);
        model.PhoneNumber.Should().Be(phoneNumber);

        sessionServiceMock.Verify(s => s.Set(It.IsAny<ProviderContactSessionModel>()), Times.Never());
    }
}
