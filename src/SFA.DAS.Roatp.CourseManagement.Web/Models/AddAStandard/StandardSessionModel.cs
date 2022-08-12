﻿using SFA.DAS.Roatp.CourseManagement.Domain.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class StandardSessionModel
    {
        public int LarsCode { get; set; }
        public bool IsConfirmed { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsPageUrl { get; set; }
        public string StandardInfoUrl { get; set; }
        public LocationOption LocationOption { get; set; }
        public bool? HasNationalDeliveryOption { get; set; }
    }
}
