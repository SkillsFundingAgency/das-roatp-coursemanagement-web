using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public class BreadcrumbsViewModel
{
    public IList<BreadcrumbItemModel> Items { get; } = new List<BreadcrumbItemModel>();

    public BreadcrumbsViewModel(IOptions<ProviderSharedUIConfiguration> providerConfiguration, IUrlHelper urlHelper, int ukprn, LearningType? learningType, IEnumerable<string> breadcrumbsToAdd)
    {
        AddDashboardLink(providerConfiguration);
        AddReviewYourDetailsLink(urlHelper);

        foreach (var breadcrumb in breadcrumbsToAdd)
        {
            AddBreadcrumb(urlHelper, ukprn, learningType, breadcrumb);
        }
    }

    public BreadcrumbsViewModel(IOptions<ProviderSharedUIConfiguration> providerConfiguration)
    {
        AddDashboardLink(providerConfiguration);
    }

    private void AddBreadcrumb(IUrlHelper urlHelper, int ukprn, LearningType? learningType, string breadcrumb)
    {
        switch (breadcrumb)
        {
            case BreadcrumbNames.ManageApprenticeships:
                AddItem(BreadcrumbNames.ManageApprenticeships, urlHelper.RouteUrl(RouteNames.ViewStandards, new { ukprn }));
                break;

            case BreadcrumbNames.ManageShortCourses:
                AddItem(BreadcrumbNames.ManageShortCourses, urlHelper.RouteUrl(RouteNames.ManageShortCourses, new { ukprn, learningType }));
                break;
        }
    }

    public void AddDashboardLink(IOptions<ProviderSharedUIConfiguration> providerConfiguration)
    {
        AddItem(BreadcrumbNames.ProviderDashboard, providerConfiguration.Value.DashboardUrl);
    }

    public void AddReviewYourDetailsLink(IUrlHelper urlHelper)
    {
        AddItem(BreadcrumbNames.ReviewYourDetails, urlHelper.RouteUrl(RouteNames.ReviewYourDetails));
    }

    public void AddItem(string description, string url)
    {
        Items.Add(new BreadcrumbItemModel(description, url));
    }
}
