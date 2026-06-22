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
        string manageCoursesLink,
        int ukprn
    )
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

        var sessionModel = new ProviderContactSessionModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber,
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
            .AddUrlForRoute(RouteNames.SelectCourseType, manageCoursesLink);

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.ContactDetailsSaved(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as AddProviderContactCompleteViewModel;
        model.EmailAddress.Should().Be(email);
        model.PhoneNumber.Should().Be(phoneNumber);
        model.ManageCoursesUrl.Should().Be(manageCoursesLink);
        model.ShowBoth.Should().Be(true);
        model.ShowEmailOnly.Should().Be(false);
        model.ShowPhoneOnly.Should().Be(false);
        model.ShowStandards.Should().BeTrue();
        model.ShowApprenticeshipUnits.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ProviderContactSessionModel)), Times.Once);
    }

    [Test, MoqAutoData]
    public void Get_CoursesReturnedInSession_PopulateCheckedCourseLists(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ProviderContactCompleteController sut,
        string manageCoursesLink,
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
            .AddUrlForRoute(RouteNames.SelectCourseType, manageCoursesLink);

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.ContactDetailsSaved(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as AddProviderContactCompleteViewModel;
        model.CheckedStandards.Courses.First().Should().Be("Test Standard (level 2)");
        model.CheckedApprenticeshipUnits.Courses.First().Should().Be("Test Apprenticeship Unit (level 2)");
    }

    [Test]
    [MoqInlineAutoData(true, "~/Views/AddProviderContact/ProviderContactAdded.cshtml")]
    [MoqInlineAutoData(false, "~/Views/AddProviderContact/ProviderContactAddedNoCourse.cshtml")]
    public void Get_UpdateExistingStandardsIsTrueOrFalse_ReturnsCorrectView(
        bool updateExistingStandards,
        string expectedViewName,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ProviderContactCompleteController sut,
        List<ProviderContactStandardModel> standards,
        int ukprn
    )
    {
        var email = "test@test.com";

        var sessionModel = new ProviderContactSessionModel
        {
            EmailAddress = email,
            Standards = standards,
            UpdateExistingStandards = updateExistingStandards,
        };

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.ContactDetailsSaved(ukprn);

        var viewResult = result as ViewResult;

        viewResult!.ViewName.Should().Be(expectedViewName);
    }

    [Test, MoqAutoData]
    public void Get_ModelInSession_Email_Only_PopulatesExpectedModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ProviderContactCompleteController sut,
        List<ProviderContactStandardModel> standards,
        int ukprn
    )
    {
        var email = "test@test.com";

        var sessionModel = new ProviderContactSessionModel
        {
            EmailAddress = email,
            Standards = standards
        };

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.ContactDetailsSaved(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as AddProviderContactCompleteViewModel;
        model.ShowBoth.Should().Be(false);
        model.ShowEmailOnly.Should().Be(true);
        model.ShowPhoneOnly.Should().Be(false);
    }

    [Test, MoqAutoData]
    public void Get_ModelInSession_Phone_Only_PopulatesExpectedModel(
       [Frozen] Mock<ISessionService> sessionServiceMock,
       [Greedy] ProviderContactCompleteController sut,
       List<ProviderContactStandardModel> standards,
       int ukprn
   )
    {
        var phoneNumber = "123445";

        var sessionModel = new ProviderContactSessionModel
        {
            PhoneNumber = phoneNumber,
            Standards = standards
        };

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.ContactDetailsSaved(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as AddProviderContactCompleteViewModel;
        model.ShowBoth.Should().Be(false);
        model.ShowEmailOnly.Should().Be(false);
        model.ShowPhoneOnly.Should().Be(true);
    }
}