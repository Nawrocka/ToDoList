namespace ToDoList1.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
   // using Microsoft.AspNet.Identity.EntityFramework;
    using ToDoList1.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ToDoList1.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ToDoList1.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            AddUsers(context);
        }

        void AddUsers(ToDoList1.Models.ApplicationDbContext context)
        {
            var user = new ApplicationUser { UserName = "user1@email.com" }; //na razie na sztywno
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            um.Create(user, "password");
        }

    }
}
