using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ESquare.WebAPI.Controllers
{
    /// <summary>
    /// Just for viewing claims
    /// </summary>
    [Authorize]
    [RoutePrefix("api/claims")]
    public class ClaimsController : BaseController
    {
        public IHttpActionResult GetClaims()
        {
            var identity = User.Identity as ClaimsIdentity;

            var claims = from c in identity.Claims
                         select new
                         {
                             subject = c.Subject.Name,
                             type = c.Type,
                             value = c.Value
                         };

            return Ok(claims);
        }

    }
}
