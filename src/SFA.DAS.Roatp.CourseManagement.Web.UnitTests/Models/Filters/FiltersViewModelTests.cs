using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.FilterComponents;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Filters;

[TestFixture]
public class FiltersViewModelTests
{
    [Test]
    public void ShowFilterOptions_WhenNoClearSections_ThenIsFalse()
    {
        var model = new FiltersViewModel { Route = "route" };

        model.ShowFilterOptions.Should().BeFalse();
    }

    [Test]
    public void ShowFilterOptions_WhenClearSectionsExist_ThenIsTrue()
    {
        var model = new FiltersViewModel
        {
            Route = "route",
            ClearFilterSections =
            [
                new ClearFilterSectionViewModel
                {
                    FilterType = FilterService.FilterType.SearchTerm,
                    Title = "Provider name",
                    Items = [new ClearFilterItemViewModel { DisplayText = "Beacon", ClearLink = "/" }]
                }
            ]
        };

        model.ShowFilterOptions.Should().BeTrue();
    }

    [Test]
    public void Ukprn_WhenSet_ThenIsReturned()
    {
        var model = new FiltersViewModel
        {
            Route = "route",
            Ukprn = 10019900
        };

        model.Ukprn.Should().Be(10019900);
    }
}
