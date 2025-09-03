using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderContactTests;

[TestFixture]
public class ProviderContactCompleteControllerTests
{
    [Test, MoqAutoData]
    public void Get_ModelMissingFromSession_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ProviderContactCompleteController sut,
        int ukprn)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns((ProviderContactSessionModel)null);

        var result = sut.ContactDetailsSaved(ukprn);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ProviderContactSessionModel)), Times.Never);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public void Get_ModelInSession_PopulatesExpectedModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ProviderContactCompleteController sut,
        List<ProviderContactStandardModel> standards,
        string reviewYourDetailsLink,
        int ukprn
    )
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

        var sessionModel = new ProviderContactSessionModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber,
            Standards = standards
        };

        sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.ReviewYourDetails, reviewYourDetailsLink);

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.ContactDetailsSaved(ukprn);

        var viewResult = result as ViewResult;

        var expectedCheckedStandards = StandardDescriptionListService.BuildSelectedStandardsList(standards);

        var model = viewResult!.Model as AddProviderContactCompleteViewModel;
        model!.BackUrl.Should().BeNull();
        model.EmailAddress.Should().Be(email);
        model.PhoneNumber.Should().Be(phoneNumber);
        model.CheckedStandards.Should().BeEquivalentTo(expectedCheckedStandards);
        model.ReviewYourDetailsUrl.Should().Be(reviewYourDetailsLink);
        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ProviderContactSessionModel)), Times.Once);
    }
}