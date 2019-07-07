﻿// <auto-generated />
using System;
using Business.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    [DbContext(typeof(FarmMasterContext))]
    [Migration("20190707035550_ContactNamesToFullName")]
    partial class ContactNamesToFullName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Business.Model.Contact", b =>
                {
                    b.Property<int>("ContactId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("FullName")
                        .IsRequired();

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("ContactId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("Business.Model.EnumRolePermission", b =>
                {
                    b.Property<int>("EnumRolePermissionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(75);

                    b.Property<string>("InternalName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("EnumRolePermissionId");

                    b.HasIndex("InternalName")
                        .IsUnique();

                    b.ToTable("EnumRolePermissions");

                    b.HasData(
                        new
                        {
                            EnumRolePermissionId = 1,
                            Description = "Edit Contacts",
                            InternalName = "edit_contacts"
                        },
                        new
                        {
                            EnumRolePermissionId = 2,
                            Description = "View Contacts",
                            InternalName = "view_contacts"
                        },
                        new
                        {
                            EnumRolePermissionId = 3,
                            Description = "Edit Roles",
                            InternalName = "edit_roles"
                        },
                        new
                        {
                            EnumRolePermissionId = 4,
                            Description = "View Roles",
                            InternalName = "view_roles"
                        },
                        new
                        {
                            EnumRolePermissionId = 5,
                            Description = "Edit Users",
                            InternalName = "edit_users"
                        },
                        new
                        {
                            EnumRolePermissionId = 6,
                            Description = "View Users",
                            InternalName = "view_users"
                        },
                        new
                        {
                            EnumRolePermissionId = 7,
                            Description = "Assign Roles",
                            InternalName = "assign_roles"
                        });
                });

            modelBuilder.Entity("Business.Model.MapRolePermissionToRole", b =>
                {
                    b.Property<int>("MapRolePermissionToRoleId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EnumRolePermissionId");

                    b.Property<int>("RoleId");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("MapRolePermissionToRoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("EnumRolePermissionId", "RoleId")
                        .IsUnique();

                    b.ToTable("MapRolePermissionToRoles");
                });

            modelBuilder.Entity("Business.Model.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<int>("HierarchyOrder");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int?>("ParentRoleId");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("RoleId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentRoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Business.Model.Telephone", b =>
                {
                    b.Property<int>("TelephoneId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContactId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasMaxLength(15);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("TelephoneId");

                    b.HasIndex("ContactId");

                    b.ToTable("Telephones");
                });

            modelBuilder.Entity("Business.Model.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContactId");

                    b.Property<int?>("RoleId");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("UserLoginInfoId");

                    b.Property<int>("UserPrivacyId");

                    b.HasKey("UserId");

                    b.HasIndex("ContactId")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.HasIndex("UserLoginInfoId");

                    b.HasIndex("UserPrivacyId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Business.Model.UserLoginInfo", b =>
                {
                    b.Property<int>("UserLoginInfoId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PassHash")
                        .IsRequired()
                        .HasMaxLength(60);

                    b.Property<string>("Salt")
                        .IsRequired();

                    b.Property<string>("SessionToken");

                    b.Property<DateTimeOffset>("SessionTokenExpiry");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(75);

                    b.HasKey("UserLoginInfoId");

                    b.ToTable("UserLoginInfo");
                });

            modelBuilder.Entity("Business.Model.UserPrivacy", b =>
                {
                    b.Property<int>("UserPrivacyId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EmailVerificationToken");

                    b.Property<bool>("HasVerifiedEmail");

                    b.Property<int>("PrivacyPolicyVersionAgreedTo");

                    b.Property<int>("TermsOfServiceVersionAgreedTo");

                    b.HasKey("UserPrivacyId");

                    b.ToTable("UserPrivacy");
                });

            modelBuilder.Entity("Business.Model.MapRolePermissionToRole", b =>
                {
                    b.HasOne("Business.Model.EnumRolePermission", "EnumRolePermission")
                        .WithMany()
                        .HasForeignKey("EnumRolePermissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Business.Model.Role", "Role")
                        .WithMany("Permissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Business.Model.Role", b =>
                {
                    b.HasOne("Business.Model.Role", "ParentRole")
                        .WithMany()
                        .HasForeignKey("ParentRoleId");
                });

            modelBuilder.Entity("Business.Model.Telephone", b =>
                {
                    b.HasOne("Business.Model.Contact", "Contact")
                        .WithMany("PhoneNumbers")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Business.Model.User", b =>
                {
                    b.HasOne("Business.Model.Contact", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Business.Model.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("Business.Model.UserLoginInfo", "UserLoginInfo")
                        .WithMany()
                        .HasForeignKey("UserLoginInfoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Business.Model.UserPrivacy", "UserPrivacy")
                        .WithOne("User")
                        .HasForeignKey("Business.Model.User", "UserPrivacyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
