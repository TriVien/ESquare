using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ESquare.Common.Constants;
using ESquare.DTO;
using ESquare.DTO.Identity;
using ESquare.Entity.Identity;
using ESquare.Infrastructure.Multitenancy;
using Microsoft.Owin;
using SaasKit.Multitenancy;

namespace ESquare.Infrastructure.Identity
{
    /// <summary>
    /// Provides additional claims for a user on the fly
    /// </summary>
    public static class ExtendedClaimsProvider
    {
        public static IEnumerable<Claim> GetClaims(ApplicationUserDto user)
        {
            var claims = new List<Claim>();

            // any logic here to determine claim

            // then add claims for user
             //claims.Add(CreateClaim("CanDoSomething", "1"));
             //claims.Add(CreateClaim("AdditionalInfo", "AAA"));

            return claims;
        }

        public static Claim GetTenantClaim(IOwinContext owinContext)
        {
            object tenantContextTemp;
            var success = owinContext.Environment.TryGetValue("saaskit:tenantContext", out tenantContextTemp);
            if (success && tenantContextTemp != null)
            {
                var tenant = ((TenantContext<Tenant>)tenantContextTemp).Tenant;
                return CreateClaim(IdentityClaimNames.TENANT_ID, tenant.Id);
            }

            return null;
        }

        public static Claim CreateClaim(string type, string value)
        {
            return new Claim(type, value, ClaimValueTypes.String);
        }

    }
}
