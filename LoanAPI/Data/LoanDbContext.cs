using Microsoft.EntityFrameworkCore;
using LoanAPI.Models;
using LoanCalculator.API.Models;

namespace LoanAPI.Data
{
    public class LoanDbContext : DbContext
    {
        public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options)
        {
        }

        public DbSet<CustomerTbl> CustomerTbl { get; set; }
        public DbSet<InventoryTbl> InventoryTbl { get; set; }
        public DbSet<CustomerDtl> CustomerDtl { get; set; }
        public DbSet<LoanDetail> LoanDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Additional model configurations if needed
            modelBuilder.Entity<LoanDetail>()
                .Property(l => l.LoanAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<LoanDetail>()
                .Property(l => l.Principal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<LoanDetail>()
                .Property(l => l.Interest)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<LoanDetail>()
                .Property(l => l.Insurance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CustomerTbl>().ToTable("CustomerTbl");
            modelBuilder.Entity<InventoryTbl>().ToTable("InventoryTbl");
            modelBuilder.Entity<CustomerDtl>().ToTable("CustomerDtl");


            modelBuilder.Entity<CustomerTbl>()
                .HasOne(c => c.Inventory)
                .WithMany(i => i.Customers)
                .HasForeignKey(c => c.UnitID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CustomerDtl>()
                .HasOne(cd => cd.Customer)
                .WithMany(c => c.CustomerDtl)
                .HasForeignKey(cd => cd.CustomerNo)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
