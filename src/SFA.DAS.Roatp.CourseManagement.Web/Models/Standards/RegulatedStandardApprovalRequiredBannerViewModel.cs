namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

public class MissingInfoBannerViewModel(bool isRegulatedForProvider, bool hasLocations, bool? isApprovedByRegulator)
{
    public bool IsRegulatedForProvider { get; } = isRegulatedForProvider;
    public bool HasLocations { get; } = hasLocations;
    public bool? IsApprovedByRegulator { get; } = isApprovedByRegulator;
    public MissingInfo? MissingInformationType => SetMissingInfoType();
    private MissingInfo? SetMissingInfoType()
    {
        if (HasLocations && IsRegulatedForProvider && IsApprovedByRegulator != null && (bool)!IsApprovedByRegulator)
            return MissingInfo.NotApproved;
        if (!HasLocations && IsRegulatedForProvider && IsApprovedByRegulator != null &&
            (bool)!IsApprovedByRegulator)
            return MissingInfo.LocationMissingAndNotApproved;
        if (!HasLocations)
            return MissingInfo.LocationMissing;
        return null;
    }
    public enum MissingInfo
    {
        LocationMissing,
        NotApproved,
        LocationMissingAndNotApproved
    }
}
