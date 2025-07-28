using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAsset_DDLA.Controllers;
[AllowAnonymous]
public class WelcomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
