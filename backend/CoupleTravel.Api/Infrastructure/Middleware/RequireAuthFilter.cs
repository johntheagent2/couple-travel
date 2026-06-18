using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoupleTravel.Api.Infrastructure.Middleware;

public class RequireAuthFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Session.GetString("userId") is null)
            context.Result = new UnauthorizedObjectResult(new { error = "Not authenticated" });
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
