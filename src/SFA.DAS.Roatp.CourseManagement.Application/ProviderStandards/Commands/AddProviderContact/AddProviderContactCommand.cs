using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderContact;
public class AddProviderContactCommand : IRequest
{
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public int Ukprn { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public List<int> ProviderCourseIds { get; set; } = new List<int>();

}
