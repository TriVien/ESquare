using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESquare.Infrastructure.Multitenancy;
using Owin;
using SaasKit.Multitenancy;

namespace ESquare.Infrastructure
{
    public class MultitenancyConfig
    {
        public static void Register(IAppBuilder app)
        {
            var cachedTenantResolver = new CachedTenantResolver<Tenant>(new TenantResolver(),
                t => new List<string> { t.Id },
                RequestIdentificationExtensions.FromHeader());
            app.UseMultitenancy(new TenantResolver());
        }
    }
}
