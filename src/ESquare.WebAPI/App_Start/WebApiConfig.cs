using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Elmah.Contrib.WebApi;
using Microsoft.Owin.Security.OAuth;
using ESquare.WebAPI.ExceptionHandling;
using ESquare.WebAPI.Infrastructure;
using Newtonsoft.Json.Serialization;

namespace ESquare.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Provides unhandled exception logging and handling
            // http://jasonwatmore.com/post/2014/05/03/Getting-ELMAH-to-catch-ALL-unhandled-exceptions-in-Web-API-21.aspx
            // http://www.asp.net/web-api/overview/error-handling/web-api-global-error-handling
            // https://www.jayway.com/2016/01/08/improving-error-handling-asp-net-web-api-2-1-owin/
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
            config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());

            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Configure global filters
            config.Filters.Add(new MultitenantAuthorizeAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultFileAccess",
                routeTemplate: "api/file/{accessToken}"
            );
        }
    }
}
