using ddla.ITApplication.Database.Models.ViewModels.Account;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
namespace ddla.ITApplication.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly IActivityLogger _activityLogger;
    public AccountController(IUserService userService, IActivityLogger activityLogger)
    {
        _userService = userService;
        _activityLogger = activityLogger;
    }

    //[HttpGet]
    //public async Task<IActionResult> Register()
    //{
    //    return View();
    //}

    //[HttpPost]
    //public async Task<IActionResult> Register(RegisterViewModel model)
    //{
    //    if (!ModelState.IsValid) return View(model);
    //    await _userService.Register(model);
    //    return RedirectToAction("Index", "Home");
    //}


    [HttpGet]
    public async Task<IActionResult> Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        bool isSuccess = await _userService.Login(model);
        if (!isSuccess)
        {
            ModelState.AddModelError("", "Email və ya şifrə yanlışdır");
            return View(model);
        }

        await _activityLogger.LogAsync(
            User.Identity.Name,
            $"İstifadəçi '{User.Identity.Name}' sistemə daxil oldu."
        );

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> LogOut()
    {
        await _activityLogger.LogAsync(
            User.Identity.Name,
            $"İstifadəçi '{User.Identity.Name}' sistemdən çıxdı."
        );
        await _userService.LogOut();
        return RedirectToAction("Index", "Welcome");
    }

    [HttpGet]
    public async Task<IActionResult> CreateRole()
    {
        await _userService.CreateRole();
        return RedirectToAction("Index", "Home");
    }
}
