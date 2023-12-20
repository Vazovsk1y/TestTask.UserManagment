﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestTask.DAL;

#nullable disable

namespace TestTask.DAL.Migrations
{
    [DbContext(typeof(TestTaskDbContext))]
    [Migration("20231220171531_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TestTask.Domain.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = new Guid("53bd3d14-5c0c-4c0c-b45f-365865b09804"),
                            Title = "Admin"
                        },
                        new
                        {
                            Id = new Guid("2771ba05-86d7-4fb3-9884-cf60755e56c9"),
                            Title = "User"
                        },
                        new
                        {
                            Id = new Guid("ade6e94e-73fb-4cee-9a93-ec008be89748"),
                            Title = "SuperAdmin"
                        },
                        new
                        {
                            Id = new Guid("10b2330b-89b1-4262-b9f3-9963d47e22e6"),
                            Title = "Support"
                        });
                });

            modelBuilder.Entity("TestTask.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("df27527b-7158-47e0-b55e-0d075f751170"),
                            Age = 25,
                            Email = "penis@gmail.com",
                            FullName = "Mike Vazovskiy",
                            PasswordHash = "$2a$11$N.2UL/6z7EeOqU4H4CjewOV6pU6rLT9n4LQk0BkgkWmbZ79SM5LF."
                        },
                        new
                        {
                            Id = new Guid("e5e3008f-93bb-4d44-8f33-e37f2b5e69ac"),
                            Age = 28,
                            Email = "jopka@gmail.com",
                            FullName = "Иван Иванов",
                            PasswordHash = "$2a$11$lWlWuU1qBurUrtyO2JeufusC.n0D1lQVyF9M5uZTBMB9cNO/tv.1a"
                        },
                        new
                        {
                            Id = new Guid("e56cfd2f-6619-420d-8d9a-3bcd5f8ef4e1"),
                            Age = 34,
                            Email = "govno@gmail.com",
                            FullName = "Иван Сидоров",
                            PasswordHash = "$2a$11$lxUKKVd01uxUvyeVLVQHXuK7Yj5kzH6cjhY5uh0aE.CuFiEpRQVIO"
                        });
                });

            modelBuilder.Entity("TestTask.Domain.Entities.UserRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UsersRoles");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("cb101933-ddd2-41df-ad02-adacf58bebd5"),
                            RoleId = new Guid("5abeb509-a426-4dad-90fa-bb51243943cd")
                        },
                        new
                        {
                            UserId = new Guid("cb101933-ddd2-41df-ad02-adacf58bebd5"),
                            RoleId = new Guid("c4a51d31-7dd4-440a-846a-b45b63e4075f")
                        },
                        new
                        {
                            UserId = new Guid("cb101933-ddd2-41df-ad02-adacf58bebd5"),
                            RoleId = new Guid("183e83b0-6027-4771-82c4-53a15de4bf66")
                        },
                        new
                        {
                            UserId = new Guid("74156d72-d1e7-4b7c-a7d6-858e7893dc02"),
                            RoleId = new Guid("f700d78f-dfd1-4593-9a34-cf098296b53c")
                        },
                        new
                        {
                            UserId = new Guid("74156d72-d1e7-4b7c-a7d6-858e7893dc02"),
                            RoleId = new Guid("14acbff4-2f9e-4d78-85f1-22fb875ec319")
                        },
                        new
                        {
                            UserId = new Guid("fe8482e1-4123-4c7e-97e2-50ad4192b80b"),
                            RoleId = new Guid("8df2a3d9-d479-4c5a-a8f2-e26f3a77a772")
                        },
                        new
                        {
                            UserId = new Guid("fe8482e1-4123-4c7e-97e2-50ad4192b80b"),
                            RoleId = new Guid("cabafc03-fb8c-41b6-a23f-59378ef91cd9")
                        });
                });

            modelBuilder.Entity("TestTask.Domain.Entities.UserRole", b =>
                {
                    b.HasOne("TestTask.Domain.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TestTask.Domain.Entities.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TestTask.Domain.Entities.User", b =>
                {
                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
