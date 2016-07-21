using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ESquare.Entity.Identity;

namespace ESquare.DAL.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ESquare.DAL.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ESquare.DAL.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            // Users
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

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
    }
}
