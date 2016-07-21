using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using ESquare.DAL;
using ESquare.DTO;
using ESquare.DTO.Identity;
using ESquare.Entity.Identity;

namespace ESquare.Infrastructure.Identity
{
    /// <summary>
    /// Wrapper for ASP.NET Identity UserManager
    /// </summary>
    public class ApplicationUserManager : IDisposable
    {
        private readonly UserManager<ApplicationUser> _userManager; 

        public ApplicationUserManager(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));

            // Configure validation logic for usernames
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true
            };

            // Configure email service
            userManager.EmailService = new EmailService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    // This affects the expired time of the token sent by email service
                    TokenLifespan = TimeSpan.FromHours(24)
                };
            }

            var manager = new ApplicationUserManager(userManager);
            return manager;
        }

        public IEnumerable<ApplicationUserDto> GetUsers()
        {
            return _userManager.Users.Select(CreateApplicationUserDto);
        }

        public Task<IList<string>> GetRolesAsync(string userId)
        {
            return _userManager.GetRolesAsync(userId);
        }

        public Task<IList<Claim>> GetClaimsAsync(string userId)
        {
            return _userManager.GetClaimsAsync(userId);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        {
            return _userManager.GetLoginsAsync(userId);
        }

        public async Task<ApplicationUserDto> FindByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            return CreateApplicationUserDto(user);
        }

        public async Task<ApplicationUserDto> FindByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return null;

            return CreateApplicationUserDto(user);
        }

        public async Task<ApplicationUserDto> FindAsync(string username, string password)
        {
            var user = await _userManager.FindAsync(username, password);
            if (user == null)
                return null;

            return CreateApplicationUserDto(user);
        }

        public async Task<ApplicationUserDto> FindAsync(UserLoginInfo userLoginInfo)
        {
            var user = await _userManager.FindAsync(userLoginInfo);
            if (user == null)
                return null;

            return CreateApplicationUserDto(user);
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserDto userDto, string authType)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            var userIdentity = await _userManager.CreateIdentityAsync(user, authType);
            // Add custom user claims here

            return userIdentity;
        }

        public Task<IdentityResult> CreateAsync(ApplicationUserDto userDto, string password)
        {
            var user = CreateApplicationUser(userDto);
            return _userManager.CreateAsync(user, password);
        }

        public Task<IdentityResult> CreateAsync(ApplicationUserDto userDto)
        {
            var user = CreateApplicationUser(userDto);
            return _userManager.CreateAsync(user);
        }

        public Task<IdentityResult> SetPasswordAsync(string userId, string password)
        {
            return _userManager.AddPasswordAsync(userId, password);
        }

        public Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            return _userManager.ChangePasswordAsync(userId, oldPassword, newPassword);
        }

        public Task<IdentityResult> RemovePasswordAsync(string userId)
        {
            return _userManager.RemovePasswordAsync(userId);
        }

        public Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo userLoginInfo)
        {
            return _userManager.AddLoginAsync(userId, userLoginInfo);
        }

        public Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo userLoginInfo)
        {
            return _userManager.RemoveLoginAsync(userId, userLoginInfo);
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(string userId)
        {
            return _userManager.GenerateEmailConfirmationTokenAsync(userId);
        }

        public Task SendEmailAsync(string userId, string subject, string body)
        {
            return _userManager.SendEmailAsync(userId, subject, body);
        }

        public Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            return _userManager.ConfirmEmailAsync(userId, token);
        }

        public async Task<IdentityResult> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.DeleteAsync(user);
        }

        public Task<bool> IsInRoleAsync(string userId, string role)
        {
            return _userManager.IsInRoleAsync(userId, role);
        }

        public Task<IdentityResult> AddToRoleAsync(string userId, string role)
        {
            return _userManager.AddToRoleAsync(userId, role);
        }

        public Task<IdentityResult> AddToRolesAsync(string userId, params string[] roles)
        {
            return _userManager.AddToRolesAsync(userId, roles);
        }

        public Task<IdentityResult> RemoveFromRoleAsync(string userId, string role)
        {
            return _userManager.RemoveFromRoleAsync(userId, role);
        }

        public Task<IdentityResult> RemoveFromRolesAsync(string userId, params string[] roles)
        {
            return _userManager.RemoveFromRolesAsync(userId, roles);
        }

        public Task<IdentityResult> AddClaimAsync(string userId, Claim claim)
        {
            return _userManager.AddClaimAsync(userId, claim);
        }

        public Task<IdentityResult> RemoveClaimAsync(string userId, Claim claim)
        {
            return _userManager.RemoveClaimAsync(userId, claim);
        }

        #region Helpers

        private ApplicationUserDto CreateApplicationUserDto(ApplicationUser user)
        {
            return new ApplicationUserDto
            {
                Avatar = user.Avatar,
                Id = user.Id,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                FirstName = user.FirstName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LastName = user.LastName
            };
        }

        private ApplicationUser CreateApplicationUser(ApplicationUserDto userDto)
        {
            return new ApplicationUser
            {
                Avatar = userDto.Avatar,
                PhoneNumber = userDto.PhoneNumber,
                UserName = userDto.UserName,
                FirstName = userDto.FirstName,
                Email = userDto.Email,
                LastName = userDto.LastName
            };
        }

        #endregion

        public void Dispose()
        {
            _userManager.Dispose();
        }
    }
}
