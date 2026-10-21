using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Factories;

public interface IBreadcrumbsViewModelFactory
{
    BreadcrumbsViewModel Create(IUrlHelper urlHelper, BreadcrumbsViewModelFactoryModel model);
}
