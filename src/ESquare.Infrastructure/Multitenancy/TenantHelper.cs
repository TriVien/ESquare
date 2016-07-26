using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SaasKit.Multitenancy;

namespace ESquare.Infrastructure.Multitenancy
{
    public class TenantHelper
    {
        public static Tenant GetCurrentTenant()
        {
            object tenantContextTemp;
            var success = HttpContext.Current.GetOwinContext().Environment.TryGetValue("saaskit:tenantContext", out tenantContextTemp);
            if (!success)
                return null;

            var tenant = ((TenantContext<Tenant>)tenantContextTemp).Tenant;
            return tenant;
        }
    }
}
