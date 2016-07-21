using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESquare.Common.Constants;
using Microsoft.Owin;
using SaasKit.Multitenancy;

namespace ESquare.Infrastructure.Multitenancy
{
    /// <summary>
    /// Resolves tenants based on request and cache the result in memory
    /// </summary>
    public class TenantResolver : ITenantResolver<Tenant>
    {
        private readonly IEnumerable<Tenant> _tenants = new List<Tenant>
        {
            new Tenant
            {
                Name = "Tenant 1",
                Id = "Tenant_1",
                ConnectionString = "Server=.;Database=ESquare;Trusted_Connection=True;MultipleActiveResultSets=true"
            },
            new Tenant
            {
                Name = "Tenant 2",
                Id = "Tenant_2",
                ConnectionString = "Server=.;Database=ESquare;Trusted_Connection=True;MultipleActiveResultSets=true"
            },
        };

        public Task<TenantContext<Tenant>> ResolveAsync(IDictionary<string, object> environment)
        {
            TenantContext<Tenant> context = null;

            var request = new OwinContext(environment).Request;
            var tenantId = request.Headers[HttpHeaderNames.TENANT_ID];
            if (!string.IsNullOrEmpty(tenantId))
            {
                var tenant = _tenants.SingleOrDefault(t => t.Id.Equals(tenantId, StringComparison.OrdinalIgnoreCase));
                context = new TenantContext<Tenant>(tenant);
            }

            return Task.FromResult(context);
        }
    }
}
