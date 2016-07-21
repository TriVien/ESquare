using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using ESquare.Infrastructure;
using Owin;

[assembly: OwinStartup(typeof(ESquare.WebAPI.Startup))]

namespace ESquare.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            MultitenancyConfig.Register(app);

            IdentityConfig.Register(app);
            ConfigureCors(app);

            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            //builder.RegisterWebApiFilterProvider()
            AutofacConfig.Register(builder);
            AutoMapperConfig.Register(builder);

            var container = builder.Build();
            var config = new HttpConfiguration
            {
                DependencyResolver = new AutofacWebApiDependencyResolver(container)
            };

            app.UseAutofacMiddleware(container);
            WebApiConfig.Register(config);
            SwaggerConfig.Register(config);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }

        private void ConfigureCors(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            // use this if you want to customize cors policy
            //var policy = new System.Web.Cors.CorsPolicy
            //{
            //    AllowAnyHeader = true,
            //    AllowAnyMethod = true,
            //    AllowAnyOrigin = true,
            //    SupportsCredentials = true
            //};
            //policy.ExposedHeaders.Add("x-filename"); // expose custom header in response

            
            //app.UseCors(new Microsoft.Owin.Cors.CorsOptions
            //{
            //    PolicyProvider = new Microsoft.Owin.Cors.CorsPolicyProvider
            //    {
            //        PolicyResolver = context => Task.FromResult(policy)
            //    }
            //});
        }
    }
}
