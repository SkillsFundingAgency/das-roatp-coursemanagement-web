using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.Roatp.CourseManagement.Web.Filters
{
    public class ClearSessionAttribute : ActionFilterAttribute
    {
        private readonly string _sessionKey;
        public ClearSessionAttribute(string sessionKey) =>_sessionKey = sessionKey;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Session.Remove(_sessionKey);
            base.OnActionExecuting(context);
        }
    }
}
