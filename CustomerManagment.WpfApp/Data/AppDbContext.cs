using CustomerManagment.WpfApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace CustomerManagment.WpfApp.Data
{
    public class AppDbContext : DbContext
    {
        private static string _dbPath;

        static AppDbContext()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CustomerManagement");

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _dbPath = Path.Combine(appDataPath, "CustomerManagement.db");
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerRequest> CustomerRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerName).IsRequired();
                entity.Property(e => e.CustomerEmail).IsRequired();
                entity.Property(e => e.CreateDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // One-to-many relationship with CustomerRequests
                entity.HasMany(e => e.CustomerRequests)
                      .WithOne(r => r.Customer)
                      .HasForeignKey(r => r.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure CustomerRequest entity
            modelBuilder.Entity<CustomerRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerRequestDescription).IsRequired();
                entity.Property(e => e.CreateDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.CustomerRequests)
                      .HasForeignKey(e => e.CustomerId);
            });
        }
    }
}