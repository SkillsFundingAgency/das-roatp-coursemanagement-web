using System;
using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.DeleteProviderLocation;
public class DeleteProviderLocationCommand : IRequest
{
    public int Ukprn { get; }
    public Guid Id { get; }
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public DeleteProviderLocationCommand(int ukprn, Guid id, string userId, string userDisplayName)
    {
        Ukprn = ukprn;
        Id = id;
        UserId = userId;
        UserDisplayName = userDisplayName;
    }
}
