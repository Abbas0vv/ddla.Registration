using Microsoft.AspNetCore.Mvc;

namespace ITAsset_DDLA.Controllers;

public class SharedController : Controller
{
    [Route("Shared/NotFound")]
    public IActionResult NotFound()
    {
        Response.StatusCode = 404;
        return View();
    }
}
