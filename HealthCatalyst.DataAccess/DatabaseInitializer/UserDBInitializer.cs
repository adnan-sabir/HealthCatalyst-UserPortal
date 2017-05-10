using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using HealthCatalyst.Domain.Data;

namespace HealthCatalyst.DataAccess.DatabaseInitializer
{
    public class UserDBInitializer : DropCreateDatabaseIfModelChanges<UserContext>
    {
        protected override void Seed(UserContext context)
        {
            var Users = new List<User>
            {
                new User { FirstName = "Mark", LastName = "Steyn", Address = "1234 Ridge Dr", Age = 56, Interests = "Soccer" , PictureFile = "user1.png" },
                new User { FirstName = "Steve", LastName = "Johnson", Address = "5678 Cedar Dr", Age = 41, Interests = "Football" , PictureFile = "" },
                new User { FirstName = "Tom", LastName = "Hanks", Address = "4321 Bayview circle", Age = 27, Interests = "Fishing" , PictureFile = "user3.png" }
            };
            Users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();

            base.Seed(context);
        }
    }
}