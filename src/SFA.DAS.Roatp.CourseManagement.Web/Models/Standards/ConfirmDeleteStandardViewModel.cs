using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class ConfirmDeleteStandardViewModel
    {
        public StandardInformationViewModel StandardInformation { get; set; }
        public string BackUrl { get; set; }
        public string CancelUrl { get; set; }

        public static implicit operator ConfirmDeleteStandardViewModel(GetStandardInformationQueryResult source)
            => new ConfirmDeleteStandardViewModel
            {
                StandardInformation = new StandardInformationViewModel
                {
                    CourseName = source.Title,
                    Level = source.Level,
                    IfateReferenceNumber = source.IfateReferenceNumber,
                    LarsCode = source.LarsCode,
                    RegulatorName = source.RegulatorName,
                    Sector = source.Sector,
                    Version = source.Version,
                }
            };
    }
}
