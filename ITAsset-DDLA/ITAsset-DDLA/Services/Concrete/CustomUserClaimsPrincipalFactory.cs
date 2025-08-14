using ddla.ITApplication.Database;
using ddla.ITApplication.Database.Models.DomainModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ITAsset_DDLA.Services.Concrete
{
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
            // Base identity
            var identity = await base.GenerateClaimsAsync(user);

            // Add custom user info claims
            identity.AddClaim(new Claim("FirstName", user.FirstName ?? string.Empty));
            identity.AddClaim(new Claim("LastName", user.LastName ?? string.Empty));
            identity.AddClaim(new Claim("ProfilePictureUrl", user.ProfilePictureUrl ?? string.Empty));
            identity.AddClaim(new Claim("CreatedAt", user.CreatedAt.ToString("o")));

            // Add roles as claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                if (!identity.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == role))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }

            // Add permissions as claims
            var permissions = await _context.UserPermissions
                .Include(up => up.Permission)
                .Where(up => up.UserId == user.Id)
                .Select(up => up.Permission.Type.ToString())
                .ToListAsync();

            foreach (var permission in permissions.Distinct())
            {
                if (!identity.HasClaim(c => c.Type == "Permission" && c.Value == permission))
                {
                    identity.AddClaim(new Claim("Permission", permission));
                }
            }

            return identity;
        }
    }
}
