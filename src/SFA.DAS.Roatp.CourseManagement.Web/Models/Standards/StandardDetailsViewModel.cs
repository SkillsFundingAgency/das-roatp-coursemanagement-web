using System.Data.Common;
using System.Net.Http.Formatting;
using Microsoft.AspNetCore.Http;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardDetailsViewModel : ViewModelBase
    {
        public StandardDetailsViewModel(HttpContext context) : base(context)
        {
        }
        
        public int LarsCode { get; set; }
        public string CourseName { get; set; }
        public string Level { get; set; }
        public string CourseDisplayName => $"{CourseName} (Level {Level})";
        public string IFateReferenceNumber { get; set; }
        public string Sector { get; set; }
        public string RegulatorName { get; set; }
        public string Version { get; set; }
        public bool IsRegulatorPresent => !string.IsNullOrEmpty(RegulatorName);

    }
}
