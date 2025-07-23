using ddla.ITApplication.Database.Models.ViewModels.Account;
using ddla.ITApplication.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
namespace ddla.ITApplication.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await _userService.Register(model);
        return RedirectToAction("Index", "Home");
    }


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

        return RedirectToAction("Table", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> LogOut()
    {
        await _userService.LogOut();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> CreateRole()
    {
        await _userService.CreateRole();
        return RedirectToAction("Index", "Home");
    }
}
