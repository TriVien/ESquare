using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using ESquare.Entity.Domain;
using ESquare.Entity.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ESquare.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDbContext
    {
        private static readonly ConcurrentDictionary<Tuple<string, string>, DbCompiledModel> _modelCache = new ConcurrentDictionary<Tuple<string, string>, DbCompiledModel>();

        static ApplicationDbContext()
        {
            var ensureDLLIsCopied = SqlProviderServices.Instance;
        }

        // Used by migrations only
        public ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        internal ApplicationDbContext(DbConnection connection, DbCompiledModel model)
            : base(connection, model, contextOwnsConnection: true)
        {
        }

        /// <summary>
        /// Creates a context that will access the specified tenant
        /// </summary>
        public static ApplicationDbContext Create(string tenant, string connectionString)
        {
            var tenantSchema = GetTenantSchemaName(tenant);
            var connection = new SqlConnection(connectionString);

            var compiledModel = _modelCache.GetOrAdd
                (
                    Tuple.Create(connection.ConnectionString, tenantSchema),
                    t =>
                    {
                        var modelBuilder = new DbModelBuilder();
                        modelBuilder.HasDefaultSchema(tenantSchema);

                        ConfigureIdentityEntities(modelBuilder);
                        
                        var model = modelBuilder.Build(connection);
                        return model.Compile();
                    }
                );

            return new ApplicationDbContext(connection, compiledModel);
        }

        /// <summary>
        /// Creates schema for a new tenant
        /// </summary>
        public static void ProvisionTenant(string tenant, string connectionString)
        {
            var tenantSchema = GetTenantSchemaName(tenant);

            using (var connection = new SqlConnection(connectionString))
            {
                var script = $@"IF NOT EXISTS (SELECT schema_name 
                                    FROM information_schema.schemata 
                                    WHERE schema_name = '{tenantSchema}' )
                                BEGIN
                                    EXEC sp_executesql N'CREATE SCHEMA {tenantSchema};';
                                END";

                connection.Open();

                var command = new SqlCommand(script, connection);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Migrates tenant schema to latest migration
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="connectionString"></param>
        public static void Migrate(string tenant, string connectionString)
        {
            var tenantSchema = GetTenantSchemaName(tenant);

            var migrationsConfig = new DbMigrationsConfiguration<ApplicationDbContext>
            {
                AutomaticMigrationsEnabled = false
            };

            migrationsConfig.SetSqlGenerator("System.Data.SqlClient", new SqlServerSchemaAwareMigrationSqlGenerator(tenantSchema));
            migrationsConfig.SetHistoryContextFactory("System.Data.SqlClient", (existingConnection, defaultSchema) => new HistoryContext(existingConnection, tenantSchema));
            migrationsConfig.TargetDatabase = new DbConnectionInfo(connectionString, "System.Data.SqlClient");
            migrationsConfig.MigrationsAssembly = typeof(ApplicationDbContext).Assembly;
            migrationsConfig.MigrationsNamespace = "ESquare.DAL.Migrations";

            var migrator = new DbMigrator(migrationsConfig);
            migrator.Update();
        }

        /// <summary>
        /// Seeds data for new tenant
        /// </summary>
        /// <param name="context"></param>
        public static void SeedData(ApplicationDbContext context)
        {
            // Users
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var testIsDataSeeded = userManager.FindByName("admin");
            if(testIsDataSeeded != null)
                return;

            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@domain.com",
                EmailConfirmed = true,
                FirstName = "Khang",
                LastName = "Tran"
            };

            userManager.Create(adminUser, "Admin@123");

            var superAdminUser = new ApplicationUser
            {
                UserName = "superadmin",
                Email = "superadmin@domain.com",
                EmailConfirmed = true,
                FirstName = "Khang",
                LastName = "Tran"
            };

            userManager.Create(superAdminUser, "Super@123");

            // Roles
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!roleManager.Roles.Any())
            {
                roleManager.Create(new IdentityRole { Name = "SuperAdmin" });
                roleManager.Create(new IdentityRole { Name = "Admin" });
                roleManager.Create(new IdentityRole { Name = "User" });
            }

            adminUser = userManager.FindByName("admin");
            userManager.AddToRoles(adminUser.Id, "Admin");

            superAdminUser = userManager.FindByName("superadmin");
            userManager.AddToRoles(superAdminUser.Id, "SuperAdmin", "Admin");
        }

        private static string GetTenantSchemaName(string tenant)
        {
            return Regex.Replace(tenant, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        /// <summary>
        /// Configures the ASP.NET Identity entities
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <remarks>
        /// This code is from the OnModelCreating of IdentityDbContext, need to get it out of there and suppress the OnModelCreating
        /// to provide our own mechanism of DbContext initializing for multi-tenant (seperate schemas) scenario
        /// </remarks>
        private static void ConfigureIdentityEntities(DbModelBuilder modelBuilder)
        {
            var table1 = modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            table1.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
            table1.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
            table1.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
            table1.Property(u => u.UserName).IsRequired().HasMaxLength(256).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex")
            {
                IsUnique = true
            }));
            table1.Property(u => u.Email).HasMaxLength(256);

            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new
            {
                r.UserId, r.RoleId
            }).ToTable("AspNetUserRoles");

            modelBuilder.Entity<IdentityUserLogin>().HasKey(l => new
            {
                l.LoginProvider, l.ProviderKey, l.UserId
            }).ToTable("AspNetUserLogins");

            modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaims");

            var table2 = modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
            table2.Property(r => r.Name).IsRequired().HasMaxLength(256).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("RoleNameIndex")
            {
                IsUnique = true
            }));
            table2.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
        }

        public new DbSet<T> Set<T>() where T : BaseAggregateRoot
        {
            return base.Set<T>();
        }
    }
}
