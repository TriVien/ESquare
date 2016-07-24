using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;

namespace ESquare.WebApp.Multitenancy
{
    /// <summary>
    /// Resolves tenants based on request and cache the result in memory
    /// </summary>
    public class TenantResolver : MemoryCacheTenantResolver<Tenant>
    {
        private readonly IEnumerable<Tenant> _tenants;

        public TenantResolver(IMemoryCache cache, ILoggerFactory loggerFactory, IOptions<TenantsConfiguration> tenantsConfig) : base(cache, loggerFactory)
        {
            _tenants = tenantsConfig.Value.Tenants;
        }

        /// <summary>
        /// Determines what information in the current request should be used to do a cache lookup e.g. the hostname.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override string GetContextIdentifier(HttpContext context)
        {
            return context.Request.Host.Value.ToLower();
        }

        /// <summary>
        /// Determines the identifiers (keys) used to cache the tenant context. 
        /// In our example tenants can have multiple domains, so we return each of the hostnames as identifiers.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<Tenant> context)
        {
            return context.Tenant.Hostnames;
        }

        /// <summary>
        /// Resolve a tenant context from the current request. This will only be executed on cache misses.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override Task<TenantContext<Tenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<Tenant> tenantContext = null;

            var tenant = _tenants.FirstOrDefault(t => t.Hostnames.Any(h => h.Equals(context.Request.Host.Value.ToLower())));
            if (tenant != null)
            {
                tenantContext = new TenantContext<Tenant>(tenant);
            }

            return Task.FromResult(tenantContext);
        }

        // Control the tenant cache lifetime here
        //protected override MemoryCacheEntryOptions CreateCacheEntryOptions()
        //{
        //    return new MemoryCacheEntryOptions().SetAbsoluteExpiration(new TimeSpan(0, 30, 0)); // Cache for 30 minutes
        //}
    }
}
