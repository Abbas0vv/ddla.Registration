using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ITAsset_DDLA.Services.Concrete;

public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ddlaUser>
{
    private readonly ddlaAppDBContext _context;

    public CustomUserClaimsPrincipalFactory(
        UserManager<ddlaUser> userManager,
        IOptions<IdentityOptions> optionsAccessor,
        ddlaAppDBContext context)
        : base(userManager, optionsAccessor)
    {
        _context = context;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ddlaUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Load user permissions
        var permissions = await _context.UserPermissions
            .Include(up => up.Permission)
            .Where(up => up.UserId == user.Id)
            .Select(up => up.Permission.Type.ToString())
            .ToListAsync();

        if (permissions.Any())
        {
            identity.AddClaim(new Claim("Permissions", string.Join(",", permissions)));
        }

        return identity;
    }
}