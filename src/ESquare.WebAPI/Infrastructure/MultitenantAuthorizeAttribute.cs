using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using ESquare.Common.Constants;

namespace ESquare.WebAPI.Infrastructure
{
    /// <summary>
    /// Compares current identity's tenant ID against request header's tenant ID to make sure a 
    /// request is intended for its correct tenant
    /// </summary>
    public class MultitenantAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var identity = (ClaimsIdentity)actionContext.ControllerContext.RequestContext.Principal.Identity;
            var tenantIdClaim = identity.FindFirst(x => x.Type.Equals(IdentityClaimNames.TENANT_ID, StringComparison.OrdinalIgnoreCase));
            var tenantIdHeader = actionContext.Request.Headers.GetValues(HttpHeaderNames.TENANT_ID).ToList();

            if (tenantIdClaim == null || !tenantIdHeader.Any())
                return false;

            if (!tenantIdHeader[0].Equals(tenantIdClaim.Value, StringComparison.OrdinalIgnoreCase))
                return false;

            return base.IsAuthorized(actionContext);
        }
    }
}