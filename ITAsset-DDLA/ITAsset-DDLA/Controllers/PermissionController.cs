using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAsset_DDLA.Controllers;

[Authorize]
public class PermissionController : Controller
{
    private readonly IUserService _userService;

    public PermissionController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        // Await the async method first
        var users = await _userService.GetAllUsersWithPermissions();

        // Now you can use LINQ operations on the List
        var viewModels = users.Select(u => new UserWithPermissionsViewModel
        {
            Id = u.Id,
            Username = u.Username,
            ProfilePictureUrl = u.ProfilePictureUrl,
            FullName = u.FullName,
            Permissions = u.Permissions.ToList()
        }).ToList();

        return View(viewModels);
    }
}
