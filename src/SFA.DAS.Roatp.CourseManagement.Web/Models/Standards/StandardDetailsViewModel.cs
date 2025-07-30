using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardDetailsViewModel
    {
        public const string LocationMissingText =
            "You must add a training option to this standard. It will not show on Find apprenticeship training until you do.";

        public const string NotApprovedText =
            "You must confirm if this standard has regulatory approval. It will not show on Find apprenticeship training until you do.";

        public const string LocationMissingAndNotApprovedText =
            "You must do 2 things before this standard will show on Find apprenticeship training:<ul class='govuk-list govuk-list--bullet'>" +
            "<li>confirm if this standard has regulatory approval</li>" +
            "<li>add a training option</li></ul>";

        public StandardInformationViewModel StandardInformation { get; set; }

        public StandardContactInformationViewModel ContactInformation { get; set; }

        public string EditLocationOptionUrl { get; set; }

        public List<ProviderCourseLocationViewModel> ProviderCourseLocations { get; set; }

        public List<ProviderCourseLocationViewModel> SubRegionCourseLocations { get; set; }

        public ProviderCourseLocationViewModel NationalCourseLocation { get; set; }

        public IEnumerable<IGrouping<string, ProviderCourseLocationViewModel>> Regions()
        {
            return SubRegionCourseLocations
                 .GroupBy(x => x.RegionName)
                 .OrderBy(x => x.Key);
        }

        public string LocationSummary => LocationSummaryCalculator.GetLocationSummary(NationalCourseLocation != null, ProviderCourseLocations.Any(), SubRegionCourseLocations.Any());

        public bool? IsApprovedByRegulator { get; set; }
        public string ApprovedByRegulatorStatus() => IsApprovedByRegulator switch
        {
            true => "Yes",
            false => "No",
            _ => "Unknown",
        };

        public string DeleteStandardUrl { get; set; }
        public string BackUrl { get; set; }
        public string EditContactDetailsUrl { get; set; }
        public string ConfirmRegulatedStandardUrl { get; set; }
        public string EditTrainingLocationsUrl { get; set; }
        public string EditProviderCourseRegionsUrl { get; set; }
        public bool IsRegulatedForProvider { get; set; }
        public bool HasLocations { get; set; }
        public bool StandardRequiresMoreInfo => SetMissingInfo();
        public string MissingInformationText => SetMissingInfoText();

        public static implicit operator StandardDetailsViewModel(GetStandardDetailsQueryResult standardDetails)
        {
            return new StandardDetailsViewModel
            {
                StandardInformation = new StandardInformationViewModel
                {
                    CourseName = standardDetails.CourseName,
                    Level = standardDetails.Level,
                    IfateReferenceNumber = standardDetails.IFateReferenceNumber,
                    LarsCode = standardDetails.LarsCode,
                    RegulatorName = standardDetails.RegulatorName,
                    Sector = standardDetails.Sector,
                    ApprenticeshipType = standardDetails.ApprenticeshipType,
                    IsRegulatedForProvider = standardDetails.IsRegulatedForProvider
                },
                ContactInformation = new StandardContactInformationViewModel
                {
                    StandardInfoUrl = standardDetails.StandardInfoUrl,
                    ContactUsPhoneNumber = standardDetails.ContactUsPhoneNumber,
                    ContactUsEmail = standardDetails.ContactUsEmail,
                    ContactUsPageUrl = standardDetails.ContactUsPageUrl
                },
                ProviderCourseLocations = standardDetails.ProviderCourseLocations.Where(a => a.LocationType == LocationType.Provider).Select(x => (ProviderCourseLocationViewModel)x).ToList(),
                SubRegionCourseLocations = standardDetails.ProviderCourseLocations.Where(a => a.LocationType == LocationType.Regional).Select(x => (ProviderCourseLocationViewModel)x).ToList(),
                NationalCourseLocation = standardDetails.ProviderCourseLocations.Where(a => a.LocationType == LocationType.National).Select(x => (ProviderCourseLocationViewModel)x).FirstOrDefault(),
                IsApprovedByRegulator = standardDetails.IsApprovedByRegulator,
                IsRegulatedForProvider = standardDetails.IsRegulatedForProvider,
                HasLocations = standardDetails.HasLocations
            };
        }
        private bool SetMissingInfo()
        {
            if (!HasLocations)
                return true;
            if (IsApprovedByRegulator == null)
                return false;
            if (IsRegulatedForProvider && (bool)!IsApprovedByRegulator)
                return true;
            return false;
        }

        private string SetMissingInfoText()
        {
            if (HasLocations && IsRegulatedForProvider && IsApprovedByRegulator != null && (bool)!IsApprovedByRegulator)
                return NotApprovedText;
            if (!HasLocations && IsRegulatedForProvider && IsApprovedByRegulator != null &&
                (bool)!IsApprovedByRegulator)
                return LocationMissingAndNotApprovedText;
            if (!HasLocations)
                return LocationMissingText;
            return "";
        }
    }
}
