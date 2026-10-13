using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Extensions;

public static class BreadcrumbUrlExtension
{
    public static IList<BreadcrumbItemModel>
        GenerateBreadcrumbs(this IUrlHelper urlHelper, IEnumerable<string> breadcrumbNames, int ukprn, LearningType? learningType = null)
    {
        var breadcrumbs = new List<BreadcrumbItemModel>();

        foreach (var breadcrumbName in breadcrumbNames)
        {
            string routeName;
            object routeValues;

            switch (breadcrumbName)
            {
                case BreadcrumbNames.ReviewYourDetails:
                    routeName = RouteNames.ReviewYourDetails;
                    routeValues = new { };
                    break;

                case BreadcrumbNames.ManageApprenticeships:
                    routeName = RouteNames.ViewStandards;
                    routeValues = new { ukprn };
                    break;

                case BreadcrumbNames.ManageShortCourses:
                    routeName = RouteNames.ManageShortCourses;
                    routeValues = new
                    {
                        ukprn,
                        learningType
                    };
                    break;

                default:
                    throw new ArgumentException(
                        $"Unknown breadcrumb: {breadcrumbName}",
                        nameof(breadcrumbNames));
            }

            var url = urlHelper.RouteUrl(routeName, routeValues) ?? string.Empty;

            breadcrumbs.Add(new BreadcrumbItemModel(breadcrumbName, url));
        }

        return breadcrumbs;
    }
}
