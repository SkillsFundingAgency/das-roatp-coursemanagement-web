using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public class EditNationalDeliveryOptionViewModel : ConfirmNationalProviderSubmitModel, IBrowserBackLink
    {
        [FromRoute]
        public int LarsCode { get; set; }
    }
}
