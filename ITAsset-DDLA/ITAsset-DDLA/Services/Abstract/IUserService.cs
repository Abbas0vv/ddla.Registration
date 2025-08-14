using ddla.ITApplication.Database.Models.DomainModels.Account;
using ddla.ITApplication.Database.Models.ViewModels.Account;
using ITAsset_DDLA.Database.Models.ViewModels.Admin;
namespace ddla.ITApplication.Services.Abstract;

public interface IUserService
{
    Task<List<string>> GetUserPermissionsAsync(ddlaUser user);
    Task<List<UserWithPermissionsViewModel>> GetAllUsersWithPermissions();
    Task<bool> Login(LoginViewModel model);
    Task LogOut();
    //Task Register(RegisterViewModel model);
    Task CreateRole();
}
