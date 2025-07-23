using ddla.ITApplication.Database.Models.DomainModels.Account;
using ddla.ITApplication.Database.Models.ViewModels.Account;
using ddla.ITApplication.Helpers.Enums;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ddla.ITApplication.Services.Concrete;

public class UserService : IUserService
{
    private readonly UserManager<ddlaUser> _userManager;
    private readonly SignInManager<ddlaUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const string IMAGE_PATH = "~/assets/images/Uploads/ProfilePictures/";

    public UserService(UserManager<ddlaUser> userManager, SignInManager<ddlaUser> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task CreateRole()
    {
        foreach (var item in Enum.GetValues(typeof(Role)))
        {
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = item.ToString()
            });
        }
    }

    public async Task<bool> Login(LoginViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is not null)
        {
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, lockoutOnFailure: false);
            return result.Succeeded;
        }
        return false;
    }


    public async Task LogOut()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task Register(RegisterViewModel model)
    {
        int count = await _userManager.Users.CountAsync();

        var user = new ddlaUser()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.UserName,
            Email = model.Email,
            ProfilePictureUrl = model.ProfilePicture.CreateFile(_webHostEnvironment.WebRootPath, IMAGE_PATH)
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            if (count == 0)
                await _userManager.AddToRoleAsync(user, Role.Admin.ToString());
            else
                await _userManager.AddToRoleAsync(user, Role.User.ToString());

            await _signInManager.SignInAsync(user, true);
        }
    }
}