﻿using System;
using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.UpdateProviderLocationDetails
{
    public class UpdateProviderLocationDetailsCommand : IRequest<Unit>
    {
        public int Ukprn { get; set; }
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string LocationName { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
