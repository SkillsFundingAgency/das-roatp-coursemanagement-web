using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models;

public class BreadcrumbsViewModelTests
{
    private const int Ukprn = 12345678;
    private const string DashboardUrl = "https://test/dashboard";
    private const string ReviewYourDetailsUrl = "https://test/review-your-details";
    private const string ManageApprenticeshipsUrl = "https://test/manage-apprenticeships";
    private const string ManageShortCoursesUrl = "https://test/manage-short-courses";

    [Test]
    public void WhenDashboardIsProvided_ThenDashboardBreadcrumbIsAdded()
    {
        // Arrange
        var configuration = CreateConfiguration();

        // Act
        var sut = new BreadcrumbsViewModel(configuration);

        // Assert
        sut.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel(
                    BreadcrumbNames.ProviderDashboard,
                    DashboardUrl)
            },
            options => options.WithStrictOrdering());
    }

    [Test]
    public void WhenNoBreadcrumbsAreProvided_ThenDefaultBreadcrumbsAreAdded()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var urlHelper = CreateUrlHelper();

        // Act
        var sut = new BreadcrumbsViewModel(configuration, urlHelper, Ukprn, null, new List<string>());

        // Assert
        sut.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel(
                    BreadcrumbNames.ProviderDashboard,
                    DashboardUrl),

                new BreadcrumbItemModel(
                    BreadcrumbNames.ReviewYourDetails,
                    ReviewYourDetailsUrl)
            },
            options => options.WithStrictOrdering());
    }

    [Test]
    public void WhenManageApprenticeshipsIsProvided_ThenManageApprenticeshipsBreadcrumbIsAdded()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var urlHelper = CreateUrlHelper();

        // Act
        var sut = new BreadcrumbsViewModel(configuration, urlHelper, Ukprn, null,
            new[]
            {
                BreadcrumbNames.ManageApprenticeships
            });

        // Assert
        sut.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel(
                    BreadcrumbNames.ProviderDashboard,
                    DashboardUrl),

                new BreadcrumbItemModel(
                    BreadcrumbNames.ReviewYourDetails,
                    ReviewYourDetailsUrl),

                new BreadcrumbItemModel(
                    BreadcrumbNames.ManageApprenticeships,
                    ManageApprenticeshipsUrl)
            },
            options => options.WithStrictOrdering());
    }

    [Test]
    public void WhenManageShortCoursesIsProvided_ThenManageShortCoursesBreadcrumbIsAdded()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var urlHelper = CreateUrlHelper();

        // Act
        var sut = new BreadcrumbsViewModel(configuration, urlHelper, Ukprn, LearningType.ApprenticeshipUnit,
            new[]
            {
                BreadcrumbNames.ManageShortCourses
            });

        // Assert
        sut.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel(
                    BreadcrumbNames.ProviderDashboard,
                    DashboardUrl),

                new BreadcrumbItemModel(
                    BreadcrumbNames.ReviewYourDetails,
                    ReviewYourDetailsUrl),

                new BreadcrumbItemModel(
                    BreadcrumbNames.ManageShortCourses,
                    ManageShortCoursesUrl)
            },
            options => options.WithStrictOrdering());
    }

    private static IOptions<ProviderSharedUIConfiguration> CreateConfiguration()
    {
        return Options.Create(
            new ProviderSharedUIConfiguration
            {
                DashboardUrl = DashboardUrl
            });
    }

    private static IUrlHelper CreateUrlHelper()
    {
        var urlHelper = new Mock<IUrlHelper>();

        urlHelper.
            Setup(url => url.RouteUrl(It.Is<UrlRouteContext>(context => context.RouteName == RouteNames.ReviewYourDetails)))
            .Returns(ReviewYourDetailsUrl);

        urlHelper
            .Setup(url => url.RouteUrl(It.Is<UrlRouteContext>(context => context.RouteName == RouteNames.ViewStandards)))
            .Returns(ManageApprenticeshipsUrl);

        urlHelper
            .Setup(url => url.RouteUrl(It.Is<UrlRouteContext>(context => context.RouteName == RouteNames.ManageShortCourses)))
            .Returns(ManageShortCoursesUrl);

        return urlHelper.Object;
    }
}