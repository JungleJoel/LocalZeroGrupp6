using backend.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace in_pos_server_csharp.Attributes
{
    public class UnauthorizedOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedException("You must log out to perform this action.");
            }
        }
    }
}
