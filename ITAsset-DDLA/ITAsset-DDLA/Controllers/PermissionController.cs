using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels.Account;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Database.Models.ViewModels.Admin;
using ITAsset_DDLA.Helpers.Enums;
using ITAsset_DDLA.Helpers.Extentions;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITAsset_DDLA.Controllers;

[Authorize(Roles = "Admin,SuperAdmin")]
public class PermissionController : Controller
{
    private readonly IUserService _userService;
    private readonly IActivityLogger _activityLogger;
    private readonly UserManager<ddlaUser> _userManager;
    private readonly ddlaAppDBContext _context;
    private readonly SignInManager<ddlaUser> _signInManager;

    public PermissionController(
        IUserService userService,
        UserManager<ddlaUser> userManager,
        SignInManager<ddlaUser> signInManager,
        ddlaAppDBContext context,
        IActivityLogger activityLogger)
    {
        _userService = userService;
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _activityLogger = activityLogger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersWithPermissions();

        var viewModels = users
            .Where(u => u.Status == LocalUserStatus.Active || u.Status == LocalUserStatus.Disable)
            .Select(u => new UserWithPermissionsViewModel
            {
                Id = u.Id,
                Username = u.Username,
                ProfilePictureUrl = u.ProfilePictureUrl,
                FullName = u.FullName,
                Status = u.Status,
                Permissions = u.Permissions.ToList()
            }).ToList();

        return View(viewModels);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }

        var user = await _userManager.Users
            .Include(u => u.UserPermissions)
            .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        // Group permissions by category
        var permissionGroups = new List<PermissionGroup>
    {
        new PermissionGroup
        {
            GroupName = "Əməliyyat İcazələri",
            Permissions = GetPermissionItems(user, "Operation")
        },
        new PermissionGroup
        {
            GroupName = "Anbar İcazələri",
            Permissions = GetPermissionItems(user, "Inventory")
        },
        new PermissionGroup
        {
            GroupName = "Avadanlıq İcazələri",
            Permissions = GetPermissionItems(user, "Equipment")
        }
    };

        var viewModel = new EditPermissionsViewModel
        {
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Username = user.UserName,
            ProfilePictureUrl = user.ProfilePictureUrl ?? "default.jpg",
            PermissionGroups = permissionGroups
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> DisableUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        user.Status = LocalUserStatus.Disable;
        user.UserPermissions.Clear(); // bütün icazələri sil
        await _userManager.UpdateAsync(user);

        TempData["SuccessMessage"] = "İstifadəçi deaktiv edildi və bütün icazələri silindi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // Burada hələlik sadəcə status dəyişək
        user.Status = LocalUserStatus.Delete;
        await _userManager.UpdateAsync(user);

        TempData["SuccessMessage"] = "İstifadəçi silinmiş statusa salındı.";
        return RedirectToAction(nameof(Index));
    }


    private List<PermissionItem> GetPermissionItems(ddlaUser user, string prefix)
    {
        return Enum.GetValues(typeof(PermissionType))
            .Cast<PermissionType>()
            .Where(p => p.ToString().StartsWith(prefix))
            .Select(p => new PermissionItem
            {
                Type = p,
                Description = p.GetDisplayName(),
                HasPermission = user.UserPermissions.Any(up => up.Permission.Type == p)
            })
            .ToList();
    }
    [HttpPost]
    public async Task<IActionResult> Edit(UpdatePermissionsViewModel model)
    {
        if (!ModelState.IsValid || model.Permissions == null)
        {
            TempData["ErrorMessage"] = "Xəta baş verdi!";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.Users
            .Include(u => u.UserPermissions)
            .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Id == model.UserId);

        if (user == null)
        {
            return NotFound();
        }

        // Get all existing permissions from database
        var allPermissions = await _context.Permissions.ToListAsync();

        foreach (var permissionUpdate in model.Permissions)
        {
            // Find the permission in database
            var permission = allPermissions.FirstOrDefault(p => p.Type == permissionUpdate.Type);
            if (permission == null) continue;

            // Check if user currently has this permission
            var userPermission = user.UserPermissions?
                .FirstOrDefault(up => up.PermissionId == permission.Id);

            if (permissionUpdate.IsSelected)
            {
                // Add permission if not already exists
                if (userPermission == null)
                {
                    user.UserPermissions ??= new List<UserPermission>();
                    user.UserPermissions.Add(new UserPermission
                    {
                        PermissionId = permission.Id,
                        UserId = user.Id
                    });
                }
            }
            else
            {
                // Remove permission if exists
                if (userPermission != null)
                {
                    _context.UserPermissions.Remove(userPermission);
                }
            }
        }

        try
        {
            await _context.SaveChangesAsync();
            var updatedUser = await _userManager.FindByIdAsync(user.Id);
            await _signInManager.RefreshSignInAsync(updatedUser);
            var addedPermissions = string.Join(", ", user.UserPermissions.Select(p => p.Permission.Type));

            await _activityLogger.LogAsync(
                User.Identity.Name,
                $"İstifadəçi '{User.Identity.Name}' '{updatedUser.UserName}' istifadəçisinə yeni icazələr əlavə etdi: {addedPermissions}"
            );


            TempData["SuccessMessage"] = "İcazələr uğurla yeniləndi!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Xəta baş verdi: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
