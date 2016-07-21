using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ESquare.Infrastructure.Identity;
using ESquare.Infrastructure.Multitenancy;

namespace ESquare.WebAPI.Controllers
{
    public abstract class BaseController : ApiController
    {
        private ApplicationUserManager _appUserManager;
        private ApplicationRoleManager _appRoleManager;
        private Tenant _currentTenant;

        protected ApplicationUserManager AppUserManager => _appUserManager ?? (_appUserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>());

        protected ApplicationRoleManager AppRoleManager => _appRoleManager ?? (_appRoleManager = Request.GetOwinContext().GetUserManager<ApplicationRoleManager>());

        protected Tenant CurrentTenant => _currentTenant ?? (_currentTenant = Request.GetTenant<Tenant>());

        protected IHttpActionResult GetIdentityErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _appUserManager?.Dispose();
                _appRoleManager?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
