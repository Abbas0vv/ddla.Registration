using ddla.ITApplication.Database.Models.DomainModels.Account;
using ITAsset_DDLA.Helpers.Enums;
using ITAsset_DDLA.Services.Abstract;
using ITAsset_DDLA.Services.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ITAsset_DDLA.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly LdapService _ldapService;
    private readonly IExcelService _excelService;

    public UsersController(IExcelService excelService)
    {
        _ldapService = new LdapService("LDAP://dc01.ddla.local/OU=contact,DC=ddla,DC=local", "DDLA\\ldapuser", "FBefONQ2JMUMVpz4yLYM");
        _excelService = excelService;
    }

    public IActionResult Index()
    {
        try
        {
            var users = _ldapService.GetLdapUsers();
            return View(users);
        }
        catch (System.Runtime.InteropServices.COMException)
        {
            return RedirectToAction("LdapConnectionFailed", "Error");
        }
    }

    [HttpGet]
    public IActionResult ExportToExcel()
    {
        var users = _ldapService.GetLdapUsers();
        var fileContent = _excelService.ExportUsersToExcel(users);

        return File(fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Istifadeciler.xlsx");
    }

}
