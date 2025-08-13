using ddla.ITApplication.Database.Models.ViewModels.Account;
using ITAsset_DDLA.Database.Models.ViewModels.Admin;
namespace ddla.ITApplication.Services.Abstract;

public interface IUserService
{
    Task<List<UserWithPermissionsViewModel>> GetAllUsersWithPermissions();
    Task<bool> Login(LoginViewModel model);
    Task LogOut();
    //Task Register(RegisterViewModel model);
    Task CreateRole();
}
