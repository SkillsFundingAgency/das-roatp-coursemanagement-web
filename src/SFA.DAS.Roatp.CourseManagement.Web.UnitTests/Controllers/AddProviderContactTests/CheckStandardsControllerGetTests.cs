using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit4;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderContactTests;

[TestFixture]
public class CheckStandardsControllerGetTests
{
    [Test, MoqAutoData]
    public void Get_ModelMissingFromSession_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckStandardsController sut,
        int ukprn)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns((ProviderContactSessionModel)null);

        var result = sut.CheckStandards(ukprn);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public void Get_ModelInSession_PopulatesExpectedModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckStandardsController sut,
        string reviewYourDetailsLink,
        string changeEmailPhoneUrl,
        string changeSelectedStandardsUrl,
        int ukprn
    )
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

        var sessionModel = new ProviderContactSessionModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber,
            UpdateExistingStandards = true,
            Standards = new List<ProviderContactStandardModel>()
            {
                new ProviderContactStandardModel
                {
                    CourseName = "Test Standard",
                    Level = 2,
                    CourseType = CourseType.Apprenticeship,
                    IsSelected = true
                },
                new ProviderContactStandardModel
                {
                    CourseName = "Test Apprenticeship Unit",
                    Level = 2,
                    CourseType = CourseType.ShortCourse,
                    IsSelected = true
                }
            }
        };

        sut.AddDefaultContextWithUser().AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ReviewYourDetails, reviewYourDetailsLink)
            .AddUrlForRoute(RouteNames.AddProviderContactDetails, changeEmailPhoneUrl)
            .AddUrlForRoute(RouteNames.AddProviderContactSelectStandardsForUpdate, changeSelectedStandardsUrl);

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.CheckStandards(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as ProviderContactCheckStandardsViewModel;
        model!.EmailAddress.Should().Be(email);
        model.PhoneNumber.Should().Be(phoneNumber);
        model.ReviewYourDetailsUrl.Should().Be(reviewYourDetailsLink);
        model.ChangeEmailPhoneUrl.Should().Be(changeEmailPhoneUrl);
        model.ChangeSelectedStandardsUrl.Should().Be(changeSelectedStandardsUrl);
        model.ShowStandards.Should().BeTrue();
        model.ShowApprenticeshipUnits.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void WhenApprenticeshipAndShortCourseCourseTypesAreReturnedInSession_ThenPopulateCheckedCourseLists(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckStandardsController sut,
        string reviewYourDetailsLink,
        string changeEmailPhoneUrl,
        string changeSelectedStandardsUrl,
        int ukprn
    )
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

        var sessionModel = new ProviderContactSessionModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber,
            UpdateExistingStandards = true,
            Standards = new List<ProviderContactStandardModel>()
            {
                new ProviderContactStandardModel
                {
                    CourseName = "Test Standard",
                    Level = 2,
                    CourseType = CourseType.Apprenticeship,
                    IsSelected = true
                },
                new ProviderContactStandardModel
                {
                    CourseName = "Test Apprenticeship Unit",
                    Level = 2,
                    CourseType = CourseType.ShortCourse,
                    IsSelected = true
                }
            }
        };

        sut.AddDefaultContextWithUser().AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ReviewYourDetails, reviewYourDetailsLink)
            .AddUrlForRoute(RouteNames.AddProviderContactDetails, changeEmailPhoneUrl)
            .AddUrlForRoute(RouteNames.AddProviderContactSelectStandardsForUpdate, changeSelectedStandardsUrl);

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.CheckStandards(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as ProviderContactCheckStandardsViewModel;
        model.CheckedStandards.Courses.First().Should().Be("Test Standard (level 2)");
        model.CheckedApprenticeshipUnits.Courses.First().Should().Be("Test Apprenticeship Unit (level 2)");
    }
}