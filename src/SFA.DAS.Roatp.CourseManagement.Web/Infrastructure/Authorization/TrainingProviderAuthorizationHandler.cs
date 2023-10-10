using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries.GetProviderAccount;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization
{
    /// <summary>
    /// Interface to define contracts related to Training Provider Authorization Handlers.
    /// </summary>
    public interface ITrainingProviderAuthorizationHandler
    {
        /// <summary>
        /// Contract to check is the logged in Provider is a valid Training Provider. 
        /// </summary>
        /// <param name="context">AuthorizationHandlerContext.</param>
        /// <returns>boolean.</returns>
        Task<bool> IsProviderAuthorized(AuthorizationHandlerContext context);
    }

    ///<inheritdoc cref="ITrainingProviderAuthorizationHandler"/>
    public class TrainingProviderAuthorizationHandler : ITrainingProviderAuthorizationHandler
    {
        private readonly IMediator _mediator;

        public TrainingProviderAuthorizationHandler(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> IsProviderAuthorized(AuthorizationHandlerContext context)
        {
            var ukprn = GetProviderId(context);

            //if the ukprn is invalid return false.
            if (ukprn <= 0) return false;

            var providerStatusDetails = await _mediator.Send(new GetProviderAccountStatusQuery(ukprn));

            // Condition to check if the Provider Details has permission to access Apprenticeship Services based on the property value "CanAccessApprenticeshipService" set to True.
            return providerStatusDetails is { CanAccessService: true };
        }

        private static long GetProviderId(AuthorizationHandlerContext context)
        {
            return long.TryParse(context.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn))?.Value, out var providerId)
                ? providerId
                : 0;
        }
    }
}
