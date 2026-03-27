using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LMS.Web.Filters;

public class SessionAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string? Roles { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;
        var role = session.GetString("role");
        var jwt = session.GetString("jwt");

        if (string.IsNullOrEmpty(jwt))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        if (!string.IsNullOrEmpty(Roles))
        {
            var allowedRoles = Roles.Split(',').Select(r => r.Trim());
            if (string.IsNullOrEmpty(role) || !allowedRoles.Contains(role))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
                // If Auth/AccessDenied doesn't exist, we'll redirect to Home/Index with error
                // context.Result = new RedirectToActionResult("Index", "Home", new { error = "Access Denied" });
            }
        }
    }
}
