using Microsoft.Owin;
using SaasKit.Multitenancy;

namespace ESquare.Infrastructure.Multitenancy
{
    public static class RequestIdentificationExtensions
    {
        public static RequestIdentificationStrategy FromHeader()
        {
            return env => new OwinContext(env).Request.Headers["TenantId"];
        }
    }
}