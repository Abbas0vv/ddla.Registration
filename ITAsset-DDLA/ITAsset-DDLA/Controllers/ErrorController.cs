using Microsoft.AspNetCore.Mvc;

namespace ITAsset_DDLA.Controllers;

[Route("Error")]
public class ErrorController : Controller
{
    // 500 və istisnalar
    [HttpGet("")]
    public IActionResult Index()
    {
        Response.StatusCode = 500;
        ViewBag.StatusCode = 500;
        return View("Error"); // generic view
    }

    // Status kodları (404, 403, 401, 405, ...)
    [HttpGet("{code:int}")]
    public IActionResult Status(int code)
    {
        if (code == 404) return View("NotFound");
        if (code == 403) return View("Forbidden");

        ViewBag.StatusCode = code; // qalan hamısı generic view
        return View("Error");
    }
}