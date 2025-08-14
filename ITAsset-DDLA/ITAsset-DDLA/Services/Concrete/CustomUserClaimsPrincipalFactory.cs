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
    private readonly UserManager<ddlaUser> _userManager;

    public CustomUserClaimsPrincipalFactory(
        UserManager<ddlaUser> userManager,
        IOptions<IdentityOptions> optionsAccessor,
        ddlaAppDBContext context)
        : base(userManager, optionsAccessor)
    {
        _context = context;
        _userManager = userManager;
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

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim("FirstName", user.FirstName ?? string.Empty),
            new Claim("LastName", user.LastName ?? string.Empty),
            new Claim("ProfilePictureUrl", user.ProfilePictureUrl ?? string.Empty),
            new Claim("CreatedAt", user.CreatedAt.ToString("o"))
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        identity.AddClaims(claims);
        return identity;
    }
}