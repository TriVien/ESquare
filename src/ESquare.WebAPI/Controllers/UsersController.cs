using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using ESquare.DTO;
using ESquare.DTO.Identity;
using ESquare.Infrastructure.Identity;
using ESquare.WebAPI.Infrastructure;
using ESquare.WebAPI.Models;
using ESquare.WebAPI.Models.User;

namespace ESquare.WebAPI.Controllers
{
    [MultitenantAuthorize(Roles = "Admin")]
    [RoutePrefix("api/users")]
    public class UsersController : BaseController
    {
        public IHttpActionResult GetUsers()
        {
            return Ok(AppUserManager.GetUsers().Select(CreateUserReturnModel));
        }

        [Route("{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string id)
        {
            var user = await AppUserManager.FindByIdAsync(id);

            if (user != null)
            {
                return Ok(CreateUserReturnModel(user));
            }

            return NotFound();

        }

        [Route("{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await AppUserManager.FindByUsernameAsync(username);

            if (user != null)
            {
                return Ok(CreateUserReturnModel(user));
            }

            return NotFound();

        }
       
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)

            var result = await AppUserManager.DeleteAsync(id);

            if (!result.Succeeded)
            {
                return GetIdentityErrorResult(result);
            }

            return Ok();
        }

        #region Helpers

        private UserReturnModel CreateUserReturnModel(ApplicationUserDto user)
        {
            return new UserReturnModel
            {
                Url = Url.Link("GetUserById", new { id = user.Id }),
                Id = user.Id,
                UserName = user.UserName,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Roles = AppUserManager.GetRolesAsync(user.Id).Result,
                Claims = AppUserManager.GetClaimsAsync(user.Id).Result
            };
        }

        #endregion
    }
}
