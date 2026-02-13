using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CryptoAlarmSystem.Api.Filters;

public class ValidateUserIdAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-User-Id", out var userId) || 
            string.IsNullOrWhiteSpace(userId))
        {
            context.ModelState.AddModelError("X-User-Id", "X-User-Id header is required");
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}
