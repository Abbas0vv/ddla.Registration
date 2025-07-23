using ddla.ITApplication.Database.Models.ViewModels.Account;
namespace ddla.ITApplication.Services.Abstract;

public interface IUserService
{
    Task Register(RegisterViewModel model);
    Task<bool> Login(LoginViewModel model);
    Task LogOut();
    Task CreateRole();
}
