#nullable enable annotations

using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.Abstract;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.FilterComponents;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Filters;

public sealed class FiltersViewModel
{
    public required string Route { get; set; }
    public int? Ukprn { get; set; }
    public string? FilterResultsFragment { get; set; }
    public IReadOnlyList<FilterSection> FilterSections { get; set; } = [];
    public IReadOnlyList<ClearFilterSectionViewModel> ClearFilterSections { get; set; } = [];
    public bool ShowFilterOptions => ClearFilterSections.Count > 0;
}
