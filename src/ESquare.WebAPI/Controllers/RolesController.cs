using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ESquare.DTO;
using ESquare.WebAPI.Models.Account;
using ESquare.WebAPI.Models.User;

namespace ESquare.WebAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/roles")]
    public class RolesController : BaseController
    {
        [Route("{id:guid}", Name = "GetRoleById")]
        public async Task<IHttpActionResult> GetRole(string id)
        {
            var role = await AppRoleManager.FindByIdAsync(id);

            if (role != null)
            {
                return Ok(CreateRoleReturnModel(role));
            }

            return NotFound();

        }

        [Route("", Name = "GetAllRoles")]
        public IHttpActionResult GetAllRoles()
        {
            var roles = AppRoleManager.Roles;

            return Ok(roles);
        }

        [Route("create")]
        public async Task<IHttpActionResult> Create(CreateRoleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = new IdentityRole { Name = model.Name };

            var result = await AppRoleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return GetIdentityErrorResult(result);
            }

            var locationHeader = new Uri(Url.Link("GetRoleById", new { id = role.Id }));

            return Created(locationHeader, CreateRoleReturnModel(role));

        }

        [Route("{id:guid}")]
        public async Task<IHttpActionResult> DeleteRole(string id)
        {

            var role = await AppRoleManager.FindByIdAsync(id);

            if (role != null)
            {
                var result = await AppRoleManager.DeleteAsync(role);

                if (!result.Succeeded)
                {
                    return GetIdentityErrorResult(result);
                }

                return Ok();
            }

            return NotFound();

        }

        [Route("ManageUsersInRole")]
        public async Task<IHttpActionResult> ManageUsersInRole(UsersInRoleModel model)
        {
            var role = await AppRoleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist");
                return BadRequest(ModelState);
            }

            foreach (var userId in model.EnrolledUserIds)
            {
                var appUser = await AppUserManager.FindByIdAsync(userId);

                if (appUser == null)
                {
                    ModelState.AddModelError("", $"User: {userId} does not exists");
                    continue;
                }

                if (!(await AppUserManager.IsInRoleAsync(userId, role.Name)))
                {
                    var result = await AppUserManager.AddToRoleAsync(userId, role.Name);

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", $"User: {userId} could not be added to role");
                    }

                }
            }

            foreach (var user in model.RemovedUserIds)
            {
                var appUser = await AppUserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", $"User: {user} does not exists");
                    continue;
                }

                var result = await AppUserManager.RemoveFromRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", $"User: {user} could not be removed from role");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        #region Helpers

        private RoleReturnModel CreateRoleReturnModel(IdentityRole role)
        {
            return new RoleReturnModel
            {
                Url = Url.Link("GetUserByRole", new { id = role.Id }),
                Id = role.Id,
                Name = role.Name
            };
        }

        #endregion
    }
}
