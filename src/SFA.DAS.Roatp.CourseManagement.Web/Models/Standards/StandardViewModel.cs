using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardViewModel
    {
        public int ProviderCourseId { get; set; }
        public string CourseName { get; set; }
        public int Level { get; set; }
        public bool IsImported { get; set; }
        public string CourseDisplayName { get; set; }
        public int LarsCode { get; set; }
        public string StandardUrl { get; set; }
        public string ApprovalBody { get; set; }
        public bool IsRegulatedForProvider { get; set; }
        public bool HasLocation { get; set; }
        public bool IsApprovalPending => SetIsApprovalPending();
        public bool? IsApprovedByRegulator { get; set; }
        public string ConfirmRegulatedStandardUrl { get; set; }
        public bool StandardRequiresMoreInfo => SetMissingInfo();

        public static implicit operator StandardViewModel(Standard source)
        {
            return new StandardViewModel
            {
                ProviderCourseId = source.ProviderCourseId,
                CourseName = source.CourseName,
                Level = source.Level,
                IsImported = source.IsImported,
                CourseDisplayName = source.CourseName + " (level " + source.Level + ")",
                LarsCode = source.LarsCode,
                ApprovalBody = source.ApprovalBody,
                IsApprovedByRegulator = source.IsApprovedByRegulator,
                IsRegulatedForProvider = source.IsRegulatedForProvider,
                HasLocation = source.HasLocations
            };
        }

        private bool SetMissingInfo()
        {
            return (!HasLocation) || (IsRegulatedForProvider && !IsApprovedByRegulator.GetValueOrDefault());
        }

        private bool SetIsApprovalPending()
        {
            return IsRegulatedForProvider && !IsApprovedByRegulator.GetValueOrDefault();
        }
    }
}
