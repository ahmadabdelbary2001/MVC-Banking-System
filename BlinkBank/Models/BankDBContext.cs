using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace BlinkBank.Models
{
    public class BankDBContext : IdentityDbContext<ApplicationUser>
    {
        public BankDBContext(DbContextOptions<BankDBContext> options) : base(options)
        {
        }

        
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Transactions> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Accounts>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Transactions>()
                .HasKey(t => t.Id);

            // Configure one-to-one relationship between ApplicationUser and Account
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Account)  // Each ApplicationUser has one Account
                .WithOne(b => b.Customer)  // Each Account has one ApplicationUser
                .HasForeignKey<Accounts>(a => a.ApplicationUserId)  // Foreign key in Accounts table
                .OnDelete(DeleteBehavior.Cascade);  // Optional: Cascade delete behavior


            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
