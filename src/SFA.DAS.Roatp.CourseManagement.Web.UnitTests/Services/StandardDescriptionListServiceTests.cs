using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Services;

[TestFixture]
public class StandardDescriptionListServiceTests
{
    [Test]
    public void GetDescriptionList_ListOnlySelectedStandards()
    {
        List<ProviderContactStandardModel> standards = new()
        {
            _standard1NotSelected,
            _standard2Selected,
            _standard3NotSelected,
            _standard4Selected
        };

        var expectedResults = new List<string>
        {
            "course xyz (Level 1)",
            "course xyz (Level 2)"
        };

        var generatedStandards = StandardDescriptionListService.BuildSelectedStandardsList(standards);

        generatedStandards.Should().BeEquivalentTo(expectedResults);
    }

    [Test]
    public void GetDescriptionList_ListSortedAlphabetically()
    {
        List<ProviderContactStandardModel> standards = new()
        {
            _standard2Selected,
            _standard4Selected,
            _standard5Selected,
            _standard6Selected,
            _standard7Selected
        };

        var expectedResults = new List<string>
        {
            "course abc (Level 1)",
            "course abc (Level 2)",
            "course def (Level 1)",
            "course xyz (Level 1)",
            "course xyz (Level 2)"
        };

        var generatedStandards = StandardDescriptionListService.BuildSelectedStandardsList(standards);

        generatedStandards.Should().BeEquivalentTo(expectedResults);
    }

    readonly ProviderContactStandardModel _standard1NotSelected = new()
    {
        CourseName = "course abc",
        Level = 7,
        IsSelected = false,
        ProviderCourseId = 1
    };

    readonly ProviderContactStandardModel _standard2Selected = new()
    {
        CourseName = "course xyz",
        Level = 1,
        IsSelected = true,
        ProviderCourseId = 2
    };

    readonly ProviderContactStandardModel _standard3NotSelected = new()
    {
        CourseName = "course xyz",
        Level = 3,
        IsSelected = false,
        ProviderCourseId = 4
    };

    readonly ProviderContactStandardModel _standard4Selected = new()
    {
        CourseName = "course xyz",
        Level = 2,
        IsSelected = true,
        ProviderCourseId = 4
    };

    readonly ProviderContactStandardModel _standard5Selected = new()
    {
        CourseName = "course abc",
        Level = 2,
        IsSelected = true,
        ProviderCourseId = 5
    };

    readonly ProviderContactStandardModel _standard6Selected = new()
    {
        CourseName = "course abc",
        Level = 1,
        IsSelected = true,
        ProviderCourseId = 6
    };

    readonly ProviderContactStandardModel _standard7Selected = new()
    {
        CourseName = "course def",
        Level = 1,
        IsSelected = true,
        ProviderCourseId = 7
    };
}
