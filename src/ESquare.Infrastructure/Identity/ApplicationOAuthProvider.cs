using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using ESquare.Entity.Identity;

namespace ESquare.Infrastructure.Identity
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        //private readonly string _publicClientId;

        //public ApplicationOAuthProvider(string publicClientId)
        //{
        //    if (publicClientId == null)
        //    {
        //        throw new ArgumentNullException(nameof(publicClientId));
        //    }

        //    _publicClientId = publicClientId;
        //}

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            var user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The username or password is incorrect.");
                return;
            }

            if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "User did not confirm email.");
                return;
            }

            var oAuthIdentity = await userManager.GenerateUserIdentityAsync(user, "JWT");
            oAuthIdentity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
            oAuthIdentity.AddClaims(ExtendedRolesProvider.GetRoles(oAuthIdentity));
            oAuthIdentity.AddClaim(ExtendedClaimsProvider.GetTenantClaim(context.OwinContext));

            var ticket = new AuthenticationTicket(oAuthIdentity, null);
            context.Validated(ticket);

            //ClaimsIdentity cookiesIdentity = await userManager.GenerateUserIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);
            //context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// As you notice the “ValidateClientAuthentication” is empty, we are considering the request valid always, 
        /// because in our implementation our client (e.g. AngularJS front-end) is trusted client and we do not need to validate it.
        /// </summary>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        //public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        //{
        //    if (context.ClientId == _publicClientId)
        //    {
        //        Uri expectedRootUri = new Uri(context.Request.Uri, "/");

        //        if (expectedRootUri.AbsoluteUri == context.RedirectUri)
        //        {
        //            context.Validated();
        //        }
        //    }

        //    return Task.FromResult<object>(null);
        //}

        //public static AuthenticationProperties CreateProperties(string userName)
        //{
        //    IDictionary<string, string> data = new Dictionary<string, string>
        //    {
        //        { "userName", userName }
        //    };
        //    return new AuthenticationProperties(data);
        //}
    }
}