using HealthCatalyst.Domain.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthCatalyst.DataAccess.EntityConfiguration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            //Primary key
            HasKey(u => u.Id);
            //Properties
            Property(u => u.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(u => u.FirstName).IsRequired().HasMaxLength(100).HasColumnType("nvarchar");
            Property(u => u.LastName).IsRequired().HasMaxLength(100).HasColumnType("nvarchar");
            Property(u => u.Address).HasMaxLength(200).HasColumnType("nvarchar");
            Property(u => u.Age).IsRequired();
            Property(u => u.Interests).HasMaxLength(200).HasColumnType("nvarchar");
            Property(u => u.PictureFile).HasMaxLength(200).HasColumnType("nvarchar");
            //Table
            ToTable("Users");
        }
    }
}