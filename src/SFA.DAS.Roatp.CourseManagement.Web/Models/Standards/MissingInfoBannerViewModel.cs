namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

public class MissingInfoBannerViewModel(bool isRegulatedForProvider, bool hasLocations, bool? isApprovedByRegulator)
{
    public bool HasMissingLocation { get; set; } = !hasLocations && (!isRegulatedForProvider || (bool)isApprovedByRegulator!);
    public bool HasMissingRegulatorApproval { get; set; } = hasLocations && isRegulatedForProvider && !(bool)isApprovedByRegulator!;
    public bool HasMissingLocationAndRegulatorApproval { get; set; } = !hasLocations && isRegulatedForProvider && !(bool)isApprovedByRegulator;
}
