using Microsoft.AspNetCore.Mvc;

namespace ITAsset_DDLA.Controllers;

public class ErrorController : Controller
{
    [Route("Error/404")]
    public IActionResult NotFound()
    {
        Response.StatusCode = 404;
        return View("NotFound");
    }

    [Route("Error/403")]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = 403;
        return View("AccessDenied");
    }

    [Route("Error/503")]
    public IActionResult LdapConnectionFailed()
    {
        Response.StatusCode = 503;
        return View("ConnectionFailed"); // Views/Error/ConnectionFailed.cshtml
    }

    [Route("Error/{statusCode}")]
    public IActionResult HandleError(int statusCode)
    {
        return statusCode switch
        {
            403 => RedirectToAction("AccessDenied"),
            404 => RedirectToAction("NotFound"),
            503 => RedirectToAction("LdapConnectionFailed"),
            _ => View("Error") // digər status kodları üçün ümumi səhifə
        };
    }
}
