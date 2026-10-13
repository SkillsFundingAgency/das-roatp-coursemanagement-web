using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models;

public class BreadcrumbsViewModelTests
{
    [Test]
    public void WhenItemIsAdded_ThenItemIsAddedToCollection()
    {
        // Arrange
        var sut = new BreadcrumbsViewModel();

        // Act
        sut.Add("Dashboard", "/dashboard");

        // Assert
        sut.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel("Dashboard", "/dashboard")
            },
            options => options.WithStrictOrdering());
    }

    [Test]
    public void WhenMultipleItemsAreAdded_ThenItemsAreAddedInOrder()
    {
        // Arrange
        var sut = new BreadcrumbsViewModel();

        // Act
        sut.Add("Dashboard", "/dashboard");
        sut.Add("Review your details", "/review-your-details");
        sut.Add("Manage apprenticeships", "/manage-apprenticeships");

        // Assert
        sut.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel( "Dashboard", "/dashboard"),

                new BreadcrumbItemModel("Review your details", "/review-your-details"),

                new BreadcrumbItemModel("Manage apprenticeships", "/manage-apprenticeships")
            },
            options => options.WithStrictOrdering());
    }

    [Test]
    public void WhenNullUrlIsAdded_ThenUrlIsStoredAsEmptyString()
    {
        // Arrange
        var sut = new BreadcrumbsViewModel();

        // Act
        sut.Add("Dashboard", null);

        // Assert
        sut.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel("Dashboard", string.Empty)
            },
            options => options.WithStrictOrdering());
    }

    [Test]
    public void WhenNoItemsAreAdded_ThenItemsCollectionIsEmpty()
    {
        // Arrange
        var sut = new BreadcrumbsViewModel();

        // Assert
        sut.Items.Should().BeEmpty();
    }
}