using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ESquare.Infrastructure.Identity
{
    /// <summary>
    /// Provides roles for a user based on user's claims on the fly
    /// 
    /// * Role is also a Claim which is of type Role
    /// </summary>
    public class ExtendedRolesProvider
    {
        public static IEnumerable<Claim> GetRoles(ClaimsIdentity identity)
        {
            List<Claim> claims = new List<Claim>();

            //if (identity.HasClaim(c => c.Type == "CanDoSomething" && c.Value == "1") &&
            //    identity.HasClaim(ClaimTypes.Role, "Admin"))
            //{
            //    // now you can use this role to do authorization on controllers
            //    // another way is authorizing by claims directly, you can use AuthorizationFilterAttribute, check ClaimsAuthorizationAttribute
            //    claims.Add(new Claim(ClaimTypes.Role, "TheBoss"));
            //}

            return claims;
        }
    }
}
