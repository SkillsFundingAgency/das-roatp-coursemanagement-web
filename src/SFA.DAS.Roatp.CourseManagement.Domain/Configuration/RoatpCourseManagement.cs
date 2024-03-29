﻿using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.CourseManagement.Domain.Configuration
{
    [ExcludeFromCodeCoverage]
    public class RoatpCourseManagement
    {
        public string RedisConnectionString { get; set; }
        public string DataProtectionKeysDatabase { get; set; }
        public bool UseDfESignIn { get; set; }
    }
}
