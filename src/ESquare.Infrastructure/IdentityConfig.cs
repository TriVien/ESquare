using System;
using ESquare.DAL;
using ESquare.Infrastructure.Identity;
using ESquare.Infrastructure.Multitenancy;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace ESquare.Infrastructure
{
    /// <summary>
    /// Superb articles about ASP.NET Identity 
    /// http://bitoftech.net/2015/01/21/asp-net-identity-2-with-asp-net-web-api-2-accounts-management/
    /// 
    /// For this project, our Authorization Server and Resource Server are the same server
    /// </summary>
    public class IdentityConfig
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        //public static string PublicClientId { get; private set; }

        public static void Register(IAppBuilder app)
        {
            ConfigureOAuthAuthorizationServer(app);
            ConfigureOAuthResourceServer(app);
        }

        /// <summary>
        /// The Authorization Server handles token generation
        /// 
        /// By doing this, the requester for an access token from our Authorization server will receive a signed token 
        /// which contains claims for a certain resource owner (user) and this token intended to certain Resource server (audience) as well.
        /// </summary>
        private static void ConfigureOAuthAuthorizationServer(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext<ApplicationDbContext>((i, o) =>
            {
                var tenant = TenantHelper.GetCurrentTenant();
                if (tenant == null)
                    return new ApplicationDbContext();

                ApplicationDbContext.ProvisionTenant(tenant.Name, tenant.ConnectionString);
                ApplicationDbContext.Migrate(tenant.Name, tenant.ConnectionString);
                var dbContext = ApplicationDbContext.Create(tenant.Name, tenant.ConnectionString);
                ApplicationDbContext.SeedData(dbContext);
                return dbContext;
            });

            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            //// Enable the application to use a cookie to store information for the signed in user
            //// and to use a cookie to temporarily store information about a user logging in with a third party login provider
            //app.UseCookieAuthentication(new CookieAuthenticationOptions());
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            //PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/oauth2/token"),
                Provider = new ApplicationOAuthProvider(),
                //Provider = new ApplicationOAuthProvider(PublicClientId),
                //AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                AccessTokenFormat = new CustomJwtFormat("http://localhost:8840"),
#if DEBUG
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
#endif
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }

        /// <summary>
        /// The Resource Server consumes the generated token
        /// 
        /// This will configure our API to trust tokens issued by our Authorization server only, 
        /// in our case the Authorization and Resource Server are the same server, 
        /// notice how we are providing the values for audience, and the audience secret we used to generate and issue the JSON Web Token.
        /// 
        /// By providing those values to the “JwtBearerAuthentication” middleware, 
        /// our API will be able to consume only JWT tokens issued by our trusted Authorization server, 
        /// any other JWT tokens from any other Authorization server will be rejected.
        /// </summary>
        /// <param name="app"></param>
        private static void ConfigureOAuthResourceServer(IAppBuilder app)
        {
            var issuer = "http://localhost:8840";
            string audienceId = AppSettings.AudienceId;
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(AppSettings.AudienceSecret);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }
    }
}
