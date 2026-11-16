using System;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Helpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardViewModel
    {
        public int ProviderCourseId { get; set; }
        public string CourseName { get; set; }
        public int Level { get; set; }
        public string CourseDisplayName { get; set; }
        public string LarsCode { get; set; }
        public string StandardUrl { get; set; }
        public string ApprovalBody { get; set; }
        public bool IsRegulatedForProvider { get; set; }
        public bool HasLocation { get; set; }
        public bool IsApprovalPending => SetIsApprovalPending();
        public bool? IsApprovedByRegulator { get; set; }
        public string ConfirmRegulatedStandardUrl { get; set; }
        public bool StandardRequiresMoreInfo => SetMissingInfo();
        public DateTime? LastDateStarts { get; set; }
        public bool HasLastDateStarts => CourseViewModelHelper.HasLastDateStarts(LastDateStarts);
        public bool FundingClosing => CourseViewModelHelper.IsFundingClosing(LastDateStarts);
        public bool FundingClosed => CourseViewModelHelper.IsFundingClosed(LastDateStarts);
        public string TagColour => CourseViewModelHelper.GetTagColour(FundingClosing, FundingClosed);

        public static implicit operator StandardViewModel(Standard source)
        {
            return new StandardViewModel
            {
                ProviderCourseId = source.ProviderCourseId,
                CourseName = source.CourseName,
                Level = source.Level,
                CourseDisplayName = source.DisplayName,
                LarsCode = source.LarsCode,
                ApprovalBody = source.ApprovalBody,
                IsApprovedByRegulator = source.IsApprovedByRegulator,
                IsRegulatedForProvider = source.IsRegulatedForProvider,
                HasLocation = source.HasLocations,
                LastDateStarts = source.LastDateStarts
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
