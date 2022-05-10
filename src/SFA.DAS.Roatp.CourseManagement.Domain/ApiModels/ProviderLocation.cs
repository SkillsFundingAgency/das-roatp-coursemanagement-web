﻿using System;

namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels
{
    public class ProviderLocation
    {
        public int ProviderId { get; set; }
        public string LocationName { get; set; }
        public string Postcode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
