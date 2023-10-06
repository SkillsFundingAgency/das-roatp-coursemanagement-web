using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries.GetProviderAccount;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Infrastructure.Authorization
{
    public class WhenHandlingTrainingProviderAuthorization
    {
        [Test, MoqAutoData]
        public async Task Then_The_ProviderStatus_Is_Valid_And_True_Returned(
            long ukprn,
            GetProviderAccountStatusResult apiResponse,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            TrainingProviderAllRolesRequirement requirement,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            apiResponse.CanAccessService = true;
            var claim = new Claim(ProviderClaims.ProviderUkprn, ukprn.ToString());
            var claimsPrinciple = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { claim }) });
            var context = new AuthorizationHandlerContext(new[] { requirement }, claimsPrinciple, null);
            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);


            mediator.Setup(m => m.Send(It.IsAny<GetProviderAccountStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            //Act
            var actual = await handler.IsProviderAuthorized(context);

            //Assert
            actual.Should().BeTrue();
        }

        [Test, MoqAutoData]
        public async Task Then_The_ProviderDetails_Is_InValid_And_False_Returned(
            long ukprn,
            GetProviderAccountStatusResult apiResponse,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            TrainingProviderAllRolesRequirement requirement,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            apiResponse.CanAccessService = false;
            var claim = new Claim(ProviderClaims.ProviderUkprn, ukprn.ToString());
            var claimsPrinciple = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { claim }) });
            var context = new AuthorizationHandlerContext(new[] { requirement }, claimsPrinciple, null);
            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            mediator.Setup(m => m.Send(It.IsAny<GetProviderAccountStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            //Act
            var actual = await handler.IsProviderAuthorized(context);

            //Assert
            actual.Should().BeFalse();
        }

        [Test, MoqAutoData]
        public async Task Then_The_ProviderDetails_Is_Null_And_False_Returned(
            long ukprn,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            TrainingProviderAllRolesRequirement requirement,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            var claim = new Claim(ProviderClaims.ProviderUkprn, ukprn.ToString());
            var claimsPrinciple = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { claim }) });
            var context = new AuthorizationHandlerContext(new[] { requirement }, claimsPrinciple, null);
            var responseMock = new FeatureCollection();
            var httpContext = new DefaultHttpContext(responseMock);
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            mediator.Setup(m => m.Send(It.IsAny<GetProviderAccountStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetProviderAccountStatusResult)null!);

            //Act
            var actual = await handler.IsProviderAuthorized(context);

            //Assert
            actual.Should().BeFalse();
        }
    }
}
