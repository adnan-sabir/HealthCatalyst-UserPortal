using HealthCatalyst.DataAccess.DatabaseInitializer;
using HealthCatalyst.DataAccess.EntityConfiguration;
using HealthCatalyst.Domain.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace HealthCatalyst.DataAccess
{
    public interface IContext
    {
        DbSet<User> Users { get; set; }
        DbSet<T> Set<T>() where T : class;
        int SaveChanges();
    }

    public class UserContext : DbContext, IContext
    {
        public UserContext()
            : base("name=UserContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            //Database.SetInitializer<UserContext>(new UserDBInitializer());
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}