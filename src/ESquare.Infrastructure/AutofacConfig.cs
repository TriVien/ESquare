using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ESquare.DAL;
using ESquare.Infrastructure.Multitenancy;
using ESquare.Repository;

namespace ESquare.Infrastructure
{
    public class AutofacConfig
    {
        /// <summary>
        /// Registers dependencies
        /// </summary>
        /// <param name="builder"></param>
        public static void Register(ContainerBuilder builder)
        {
            RegisterDalDependencies(builder);
            RegisterRepositoryDependencies(builder);
            RegisterServiceDependencies(builder);
        }

        private static void RegisterDalDependencies(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var tenant = TenantHelper.GetCurrentTenant();
                if (tenant == null)
                    return new ApplicationDbContext();

                var dbContext = ApplicationDbContext.Create(tenant.Name, tenant.ConnectionString);
                return dbContext;
            }).As<IDbContext>();
        }

        private static void RegisterRepositoryDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
        }

        private static void RegisterServiceDependencies(ContainerBuilder builder)
        {
            // A trick to load dependency assembly prior to do the registration
            // Just need to register for 1 class to let the assembly load, once it is loaded, 
            // it can then be used to scan for other classes and registered dynamically
            //builder.RegisterType<FakeService>.As<IFakeService>();

            RegisterDependenciesByConvention(builder, "ESquare.Service");
        }

        /// <summary>
        /// Registers dynamically all implementations with their interface
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblyName"></param>
        private static void RegisterDependenciesByConvention(ContainerBuilder builder, string assemblyName)
        {
            var neededAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.StartsWith(assemblyName)).ToArray();
            builder.RegisterAssemblyTypes(neededAssemblies)
                .AsImplementedInterfaces()
                .PreserveExistingDefaults();
        }
    }
}
