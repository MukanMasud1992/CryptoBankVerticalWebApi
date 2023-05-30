using CryptoBankVerticalWebApi.Features.Accounts.Domain;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace CryptoBankVerticalWebApi.Database
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        //public DbSet<RefreshToken> RefreshTokens { get; set; } 

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            MapUser(modelBuilder);
            MapAccounts(modelBuilder);
        }

        private void MapUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(x => x.Id);

                user.Property(u=>u.Email)
                    .IsRequired();

                user.Property(x => x.Password)
                    .IsRequired();

                user.Property(x => x.PasswordSalt)
                    .IsRequired();

                user.Property(x => x.BirthDate)
                    .IsRequired();

                user.Property(x => x.DateOfRegistration)
                    .IsRequired();

                user.Property(x => x.Role)
                    .IsRequired();

                user.Property(x => x.CreatedAt)
                    .IsRequired();

                user.Property(x => x.UpdatedAt);

                user.Property(x => x.DeleteAt);

                user.HasMany(u => u.Accounts)
               .WithOne(a => a.User)
               .HasForeignKey(a => a.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_Accounts_User");
            });
        }
        private void MapAccounts(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Account>(account =>
            {
                account.HasKey(a => a.Id);

                account.Property(a => a.Currency)
                     .IsRequired();

                account.Property(a => a.Amount)
                    .IsRequired();

                account.Property(a => a.DateOfOpening)
                    .IsRequired();

                account.Property(a => a.UserId)
                    .IsRequired();

                account.HasOne(a => a.User)
                    .WithMany(a => a.Accounts)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Accounts_User");
            });
        }
    }
}
