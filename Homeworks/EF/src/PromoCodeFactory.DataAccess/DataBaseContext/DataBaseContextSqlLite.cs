using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.DataBaseContext
{
    public class DataBaseContextSqlLite : DbContext
    {
        public DataBaseContextSqlLite(DbContextOptions<DataBaseContextSqlLite> options) : base(options) 
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Preference> Preferences { get; set; }

        public DbSet<PromoCode> PromoCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== ROLE ==========
            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(r => r.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(r => r.Description)
                      .HasMaxLength(300);
            });

            // ========== EMPLOYEE ==========
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.LastName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.AppliedPromocodesCount)
                      .IsRequired();


                entity.HasOne(e => e.Role)
                      .WithMany() 
                      .HasForeignKey("RoleId")
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Preference>(entity =>
            {
                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            // ========== CUSTOMER ==========
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(c => c.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.LastName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.Email)
                      .IsRequired()
                      .HasMaxLength(200);


                entity.HasMany(c => c.PromoCodes)
                      .WithOne() 
                      .HasForeignKey("CustomerId")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== PROMOCODE ==========
            modelBuilder.Entity<PromoCode>(entity =>
            {
                entity.Property(p => p.Code)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(p => p.ServiceInfo)
                      .HasMaxLength(500);

                entity.Property(p => p.PartnerName)
                      .HasMaxLength(200);


                entity.HasOne(p => p.PartnerManager)
                      .WithMany()
                      .HasForeignKey("PartnerManagerId") 
                      .OnDelete(DeleteBehavior.Restrict);

                // 🔹 Связь с Preference
                entity.HasOne(p => p.Preference)
                      .WithMany()
                      .HasForeignKey("PreferenceId") 
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<CustomerPreference>(entity =>
            {
                entity.HasKey(cp => new { cp.CustomerId, cp.PreferenceId });

                entity.HasOne(cp => cp.Customer)
                      .WithMany(c => c.CustomerPreferences)
                      .HasForeignKey(cp => cp.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.Preference)
                      .WithMany()
                      .HasForeignKey(cp => cp.PreferenceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }

}
