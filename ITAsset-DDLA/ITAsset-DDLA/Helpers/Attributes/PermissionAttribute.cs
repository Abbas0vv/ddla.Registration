using ITAsset_DDLA.Helpers.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace ITAsset_DDLA.Helpers.Attributes
{
    public class PermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly PermissionType _permission;

        public PermissionAttribute(PermissionType permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Əgər istifadəçi login olmayıbsa
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Claim-ləri götür
            var permissionsClaim = user.Claims.FirstOrDefault(c => c.Type == "Permissions")?.Value;

            if (string.IsNullOrEmpty(permissionsClaim))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Claim-ləri ayır və trim et
            var permissions = permissionsClaim
                .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim());

            // İcazəni yoxla (böyük/kiçik hərf fərqinə tolerant)
            bool hasPermission = permissions.Any(p =>
                string.Equals(p, _permission.ToString(), System.StringComparison.OrdinalIgnoreCase));

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
