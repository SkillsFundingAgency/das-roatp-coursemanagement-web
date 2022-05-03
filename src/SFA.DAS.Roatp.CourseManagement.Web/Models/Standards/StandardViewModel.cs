namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardViewModel 
    {
        public int ProviderCourseId { get; set; }
        public string CourseName { get; set; }
        public int Level { get; set; }
        public bool IsImported { get; set; }
        public string CourseDisplayName { get; set; }

        public static implicit operator StandardViewModel(Domain.Standards.Standard source)
        {
            return new StandardViewModel
            {
                ProviderCourseId = source.ProviderCourseId,
                CourseName = source.CourseName,
                Level = source.Level,
                IsImported = source.IsImported,
                CourseDisplayName = source.CourseName + " (Level " + source.Level + ")",
            };
        }
    }
}
