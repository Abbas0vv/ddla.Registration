using ITAsset_DDLA.Helpers.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new ChallengeResult(); // login olmayanları yönləndir
                return;
            }

            var permissions = user.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value);

            if (!permissions.Any(p => string.Equals(p, _permission.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                context.Result = new ForbidResult(); // login olub, amma icazəsi yoxdursa
            }
        }

    }
}
