﻿using CryptoBankVerticalWebApi.Features.Accounts.Domain;
using CryptoBankVerticalWebApi.Features.Auth.Model;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace CryptoBankVerticalWebApi.Database
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            MapUsers(modelBuilder);
            MapAccounts(modelBuilder);
            MapRoles(modelBuilder);
            MapRefreshTokens(modelBuilder);
        }

        private void MapUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(x => x.Id);

                user.Property(u=>u.Email)
                    .IsRequired();

                user.Property(x => x.PasswordHashAndSalt)
                    .IsRequired();

                user.Property(x => x.MemorySize)
                    .IsRequired();

                user.Property(x => x.Parallelism)
                  .IsRequired();

                user.Property(x => x.Iterations)
              .IsRequired();

                user.Property(x => x.BirthDate)
                    .IsRequired();

                user.Property(x => x.CreatedAt)
                    .IsRequired();

                user.Property(x => x.UpdatedAt);

                user.HasMany(u => u.Accounts)
               .WithOne(a => a.User)
               .HasForeignKey(a => a.UserId)
               .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void MapRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(role => 
            {
                role.HasKey(x => x.Id);

                role.Property(x => x.UserId)
                .IsRequired();

                role.Property(x => x.Name)
                .IsRequired();

                role.Property(x => x.CreatedAt)
                .IsRequired();

                role.HasOne(r => r.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void MapAccounts(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Account>(account =>
            {
                account.HasKey(a => a.Id);

                account.Property(a => a.Number)
                .IsRequired();

                account.Property(a => a.Currency)
                     .IsRequired();

                account.Property(a => a.Amount)
                    .IsRequired();

                account.Property(a => a.CreatedAt)
                    .IsRequired();

                account.Property(a => a.UserId)
                    .IsRequired();

                account.HasOne(a => a.User)
                    .WithMany(a => a.Accounts)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void MapRefreshTokens(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(refreshToken =>
            {
                refreshToken.HasKey(r => r.Id);

                refreshToken.Property(e => e.Token)
              .IsRequired()
              .HasMaxLength(1000);

                refreshToken.Property(r => r.userId)
                .IsRequired();

                refreshToken.HasOne(d => d.User)
                .WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.userId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
