using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.Roatp.CourseManagement.Web.Filters
{
    public class ClearSessionAttribute : ActionFilterAttribute
    {
        public readonly string SessionKey;
        public ClearSessionAttribute(string sessionKey) => SessionKey = sessionKey;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Session.Remove(SessionKey);
            base.OnActionExecuting(context);
        }
    }
}
