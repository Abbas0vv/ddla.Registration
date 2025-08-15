using ddla.ITApplication.Database.Models.DomainModels.Account;
using ddla.ITApplication.Database.Models.ViewModels.Account;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.ViewModels.Account;
using ITAsset_DDLA.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ddla.ITApplication.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IActivityLogger _activityLogger;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ddlaUser> _userManager;
        private readonly SignInManager<ddlaUser> _signInManager;
        private const string FOLDER_NAME = "assets/images/Uploads/ProfilePictures";

        public SettingsController(
            UserManager<ddlaUser> userManager,
            IUserService userService,
            IWebHostEnvironment webHostEnvironment,
            SignInManager<ddlaUser> signInManager,
            IActivityLogger activityLogger)
        {
            _userManager = userManager;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
            _signInManager = signInManager;
            _activityLogger = activityLogger;
        }

        public async Task<IActionResult> UpdateProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var model = new UpdateProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return View(model);
        }

        public async Task<IActionResult> MyRoles()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _userService.GetUserPermissionsAsync(user);

            // Debugging: Check permissions
            foreach (var permission in permissions)
            {
                Console.WriteLine(permission); // Or use a logger to check permissions
            }

            return Content($"User: {user.UserName} | Roles: {string.Join(", ", roles)} | Permissions: {string.Join(", ", permissions)}");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Sign out and sign back in to refresh the identity
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, isPersistent: true);

                await _activityLogger.LogAsync(User.Identity.Name, "profilini redaktə etdi");
                TempData["SuccessMessage"] = "Profil məlumatları uğurla yeniləndi";
                return RedirectToAction("UpdateProfile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("UpdateProfile", model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(UpdateProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (model.ProfilePicture != null)
            {
                user.ProfilePictureUrl = model.ProfilePicture?.CreateImageFile(_webHostEnvironment.WebRootPath, FOLDER_NAME);
                await _activityLogger.LogAsync(User.Identity.Name, "profil şəklini redaktə etdi");
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = "Profil şəkli uğurla yeniləndi";
            }

            return RedirectToAction("UpdateProfile");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Şifrə uğurla dəyişdirildi";
                await _activityLogger.LogAsync(User.Identity.Name, "şifrəsini dəyişdi");
                return RedirectToAction("UpdateProfile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}