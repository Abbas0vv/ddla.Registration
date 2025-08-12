using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels.Account;
using ddla.ITApplication.Database.Models.ViewModels.Account;
using ddla.ITApplication.Helpers.Enums;
using ddla.ITApplication.Helpers.Extentions;
using ddla.ITApplication.Services.Abstract;
using ITAsset_DDLA.Database.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ddla.ITApplication.Services.Concrete;

public class UserService : IUserService
{
    private readonly UserManager<ddlaUser> _userManager;
    private readonly SignInManager<ddlaUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ddlaAppDBContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const string IMAGE_PATH = "~/assets/images/Uploads/ProfilePictures/";

    public UserService(
        UserManager<ddlaUser> userManager, 
        SignInManager<ddlaUser> signInManager,
        RoleManager<IdentityRole> roleManager
        , IWebHostEnvironment webHostEnvironment, 
        ddlaAppDBContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }
    public async Task<List<UserWithPermissionsViewModel>> GetAllUsersWithPermissions()
    {
        // First await the Task to get the List
        var users = await _context.Users
            .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
            .ToListAsync();

        // Now you can use Select on the List
        return users.Select(u => new UserWithPermissionsViewModel
        {
            Id = u.Id,
            Username = u.UserName,
            FullName = $"{u.FirstName} {u.LastName}",
            ProfilePictureUrl = u.ProfilePictureUrl ?? "~/assets//images/Uploads/ProfilePictures/default.jpg",
            Permissions = u.UserPermissions
                .Select(up => up.Permission.Type)
                .ToList()
        }).ToList();
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
        var user = await _userManager.FindByEmailAsync(model.EmailOrName)
            ?? await _userManager.FindByNameAsync(model.EmailOrName);
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
            if (count == 1)
                await _userManager.AddToRoleAsync(user, Role.Admin.ToString());
            else
                await _userManager.AddToRoleAsync(user, Role.User.ToString());

            await _signInManager.SignInAsync(user, true);
        }
    }
}