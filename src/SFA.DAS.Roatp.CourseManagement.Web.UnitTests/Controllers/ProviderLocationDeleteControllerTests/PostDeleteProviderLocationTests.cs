using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.DeleteProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderLocationDeleteControllerTests;

[TestFixture]
public class PostDeleteProviderLocationTests
{
    [Test, MoqAutoData]
    public async Task Post_ValidCommand_RedirectsToDeleteDonePage(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        int ukprn,
        Guid id)
    {
        var tempDataMock = new Mock<ITempDataDictionary>();
        sut.TempData = tempDataMock.Object;
        sut.AddDefaultContextWithUser();

        var result = await sut.PostDeleteProviderLocation(ukprn, id);

        mediatorMock.Verify(m => m.Send(It.Is<DeleteProviderLocationCommand>(c => c.Ukprn == ukprn && c.Id == id), It.IsAny<CancellationToken>()), Times.Once);

        result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.ConfirmDeleteLocationDone);
        tempDataMock.Verify(t => t.Remove(TempDataKeys.ProviderLocationDeletedBannerTempDateKey));
        tempDataMock.Verify(t => t.Add(TempDataKeys.ProviderLocationDeletedBannerTempDateKey, true));
    }
}
