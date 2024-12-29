using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (User.Identity.IsAuthenticated)
        {
            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserName = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.Role = User.FindFirstValue(ClaimTypes.Role);
        }
        base.OnActionExecuting(context);
    }
}
