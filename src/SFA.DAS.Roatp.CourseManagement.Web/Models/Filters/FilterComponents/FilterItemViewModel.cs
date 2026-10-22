namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.FilterComponents;

public sealed class FilterItemViewModel
{
    public required string Value { get; set; }
    public required string DisplayText { get; set; }
    public string? DisplayDescription { get; set; }
    public bool IsSelected { get; set; }
}
