using ITAsset_DDLA.Services.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAsset_DDLA.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly LdapService _ldapService;

    public UsersController()
    {
        _ldapService = new LdapService("LDAP://dc01.ddla.local/OU=contact,DC=ddla,DC=local", "DDLA\\ldapuser", "FBefONQ2JMUMVpz4yLYM");
    }

    public IActionResult Index()
    {
        var users = _ldapService.GetLdapUsers();
        return View(users);
    }
}
