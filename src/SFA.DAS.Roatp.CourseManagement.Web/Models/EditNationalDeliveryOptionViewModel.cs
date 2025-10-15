﻿using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public class EditNationalDeliveryOptionViewModel : ConfirmNationalProviderSubmitModel, IBackLink
    {
        [FromRoute]
        public int LarsCode { get; set; }
    }
}
