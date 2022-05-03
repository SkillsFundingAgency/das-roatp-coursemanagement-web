﻿namespace SFA.DAS.Roatp.CourseManagement.Domain.Standards
{
    public class Standard
    {
        public int ProviderCourseId { get; set; }
        public string CourseName { get; set; }
        public int Level { get; set; }
        public bool IsImported { get; set; }
        public bool CourseDisplayName { get; set; }
    }
}