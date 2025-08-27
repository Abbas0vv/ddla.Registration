using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITAsset_DDLA.Helpers
{
    public static class HtmlHelpers
    {
        public static string IsActive(this IHtmlHelper htmlHelper, string controller, params string[] actions)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeAction = routeData.Values["action"]?.ToString();
            var routeController = routeData.Values["controller"]?.ToString();

            bool isActive = string.Equals(controller, routeController, StringComparison.OrdinalIgnoreCase)
                         && (actions.Length == 0 || actions.Contains(routeAction, StringComparer.OrdinalIgnoreCase));

            return isActive ? "active" : "";
        }

    }
}
