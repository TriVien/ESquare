using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;

namespace ESquare.Multitenancy
{
    /// <summary>
    /// Customizes the view engine, tell it where to look for specific tenant view
    /// </summary>
    /// <example>
    /// The view engine looks for view in the following locations:
    /// /Themes/Cerulean/Home/Index_tenant-1.cshtml
    /// /Themes/Cerulean/Home/Index.cshtml
    /// /Themes/Cerulean/Shared/Index_tenant-1.cshtml
    /// /Themes/Cerulean/Shared/Index.cshtml
    /// /Views/Home/Index.cshtml
    /// /Views/Shared/Index.cshtml
    /// </example>
    public class TenantViewLocationExpander : IViewLocationExpander
    {
        private const string THEME_KEY = "theme";
        private const string TENANT_KEY = "tenant";

        /// <summary>
        /// Determines the values that will be used by the view location expander. 
        /// This is where you can add information that can be used to dynamically build your view location paths
        /// </summary>
        /// <param name="context"></param>
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values[THEME_KEY] = context.ActionContext.HttpContext.GetTenant<Tenant>()?.Theme;
            context.Values[TENANT_KEY] = context.ActionContext.HttpContext.GetTenant<Tenant>()?.Name.Replace(" ", "-");
        }

        /// <summary>
        /// Preserves the default locations and add our theme locations on top to ensure that they take priority.
        /// This will only be invoked if the values returned from PopulateValues have changed since the last time the view was located
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewLocations"></param>
        /// <returns></returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            string theme;
            if (context.Values.TryGetValue(THEME_KEY, out theme))
            {
                IEnumerable<string> themeLocations = new[]
                {
                    $"/Themes/{theme}/{{1}}/{{0}}.cshtml",
                    $"/Themes/{theme}/Shared/{{0}}.cshtml"
                };

                string tenant;
                if (context.Values.TryGetValue(TENANT_KEY, out tenant))
                {
                    themeLocations = ExpandTenantLocations(tenant, themeLocations);
                }

                viewLocations = themeLocations.Concat(viewLocations);
            }


            return viewLocations;
        }

        private IEnumerable<string> ExpandTenantLocations(string tenant, IEnumerable<string> defaultLocations)
        {
            foreach (var location in defaultLocations)
            {
                yield return location.Replace("{0}", $"{{0}}_{tenant}");
                yield return location;
            }
        }
    }
}
