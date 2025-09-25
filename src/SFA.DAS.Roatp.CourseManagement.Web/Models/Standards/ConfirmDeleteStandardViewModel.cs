using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class ConfirmDeleteStandardViewModel : IBackLink
    {
        public StandardInformationViewModel StandardInformation { get; set; }

        public static implicit operator ConfirmDeleteStandardViewModel(GetStandardInformationQueryResult source)
            => new ConfirmDeleteStandardViewModel
            {
                StandardInformation = (StandardInformationViewModel)source
            };
    }
}
