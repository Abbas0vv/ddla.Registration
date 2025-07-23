using Microsoft.AspNetCore.Mvc;

namespace ITAsset_DDLA.Controllers
{
    public class WelcomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
