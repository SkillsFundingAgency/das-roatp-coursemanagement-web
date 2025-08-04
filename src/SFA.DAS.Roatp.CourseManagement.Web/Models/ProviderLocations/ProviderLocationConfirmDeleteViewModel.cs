using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

public class ProviderLocationConfirmDeleteViewModel : IBackLink
{
    public Guid NavigationId { get; set; }
    public string TrainingVenuesUrl { get; set; }
    public List<ProviderLocationStandardModel> Standards { get; set; } = new();
    public bool HasNoStandards { get; set; }
    public bool HasOneStandard { get; set; }
    public bool HasTwoOrMoreStandards { get; set; }
    public string BackUrl { get; set; }

    public static implicit operator ProviderLocationConfirmDeleteViewModel(ProviderLocation source)
    {
        var standards = source.Standards.Select(s => (ProviderLocationStandardModel)s).OrderBy(s => s.CourseDisplayName)
            .ToList();

        var numberOfStandards = source.Standards == null || source.Standards.Count == 0 ? 0 : source.Standards.Count;
        return new ProviderLocationConfirmDeleteViewModel
        {
            NavigationId = source.NavigationId,
            Standards = standards,
            HasNoStandards = numberOfStandards == 0,
            HasOneStandard = numberOfStandards == 1,
            HasTwoOrMoreStandards = numberOfStandards > 1
        };
    }
}
