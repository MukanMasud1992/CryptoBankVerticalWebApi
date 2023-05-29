using CryptoBankVerticalWebApi.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace CryptoBankVerticalWebApi.Database
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        //public DbSet<RefreshToken> RefreshTokens { get; set; } 

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<RefreshToken>(refToken =>
            //{
            //    refToken.Property(e => e.Token).IsRequired().HasMaxLength(1000);

            //    refToken.HasOne(a => a.Account)
            //    .WithMany(r => r.RefreshTokens)
            //    .HasForeignKey(a => a.AccountId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_RefreshTokens_Account");
            //});
        }
    }
}
