﻿// <auto-generated />
using System;
using CryptoBankVerticalWebApi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CryptoBankVerticalWebApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230604165540_EditFieldInRefreshToken")]
    partial class EditFieldInRefreshToken
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CryptoBankVerticalWebApi.Features.Accounts.Domain.Account", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("CryptoBankVerticalWebApi.Features.Auth.Model.RefreshToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("NextRefreshToken")
                        .HasColumnType("bigint");

                    b.Property<bool>("Revoke")
                        .HasColumnType("boolean");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<long>("userId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("userId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("CryptoBankVerticalWebApi.Features.Users.Domain.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Name")
                        .HasColumnType("integer");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("CryptoBankVerticalWebApi.Features.Users.Domain.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Iterations")
                        .HasColumnType("integer");

                    b.Property<int>("MemorySize")
                        .HasColumnType("integer");

                    b.Property<int>("Parallelism")
                        .HasColumnType("integer");

                    b.Property<string>("PasswordHashAndSalt")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CryptoBankVerticalWebApi.Features.Accounts.Domain.Account", b =>
                {
                    b.HasOne("CryptoBankVerticalWebApi.Features.Users.Domain.User", "User")
                        .WithMany("Accounts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CryptoBankVerticalWebApi.Features.Auth.Model.RefreshToken", b =>
                {
                    b.HasOne("CryptoBankVerticalWebApi.Features.Users.Domain.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("userId")
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CryptoBankVerticalWebApi.Features.Users.Domain.Role", b =>
                {
                    b.HasOne("CryptoBankVerticalWebApi.Features.Users.Domain.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CryptoBankVerticalWebApi.Features.Users.Domain.User", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("RefreshTokens");

                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
