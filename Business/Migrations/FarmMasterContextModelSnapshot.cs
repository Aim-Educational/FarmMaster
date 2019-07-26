﻿// <auto-generated />
using System;
using Business.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    [DbContext(typeof(FarmMasterContext))]
    partial class FarmMasterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Business.Model.ActionAgainstContactInfo", b =>
                {
                    b.Property<int>("ActionAgainstContactInfoId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActionType")
                        .IsRequired();

                    b.Property<string>("AdditionalInfo")
                        .HasMaxLength(150);

                    b.Property<int>("ContactAffectedId");

                    b.Property<DateTimeOffset>("DateTimeUtc");

                    b.Property<bool>("HasContactBeenInformed");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<int>("UserResponsibleId");

                    b.HasKey("ActionAgainstContactInfoId");

                    b.HasIndex("ContactAffectedId");

                    b.HasIndex("HasContactBeenInformed");

                    b.HasIndex("UserResponsibleId");

                    b.ToTable("ActionsAgainstContactInfo");
                });

            modelBuilder.Entity("Business.Model.AnimalCharacteristic", b =>
                {
                    b.Property<int>("AnimalCharacteristicId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CalculatedType")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasComputedColumnSql("\"CalculatedType\"::json->'__TYPE'");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasMaxLength(65535);

                    b.Property<int>("ListId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(75);

                    b.HasKey("AnimalCharacteristicId");

                    b.HasIndex("ListId");

                    b.ToTable("AnimalCharacteristic");
                });

            modelBuilder.Entity("Business.Model.AnimalCharacteristicList", b =>
                {
                    b.Property<int>("AnimalCharacteristicListId")
                        .ValueGeneratedOnAdd();

                    b.HasKey("AnimalCharacteristicListId");

                    b.ToTable("AnimalCharacteristicList");
                });

            modelBuilder.Entity("Business.Model.Breed", b =>
                {
                    b.Property<int>("BreedId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BreedSocietyId");

                    b.Property<int?>("CharacteristicListId");

                    b.Property<bool>("IsRegisterable");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(75);

                    b.Property<int>("SpeciesId");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("BreedId");

                    b.HasIndex("BreedSocietyId");

                    b.HasIndex("CharacteristicListId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("SpeciesId");

                    b.ToTable("Breeds");
                });

            modelBuilder.Entity("Business.Model.Contact", b =>
                {
                    b.Property<int>("ContactId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContactType")
                        .IsRequired();

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<bool>("IsAnonymous");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("ContactId");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("Business.Model.Email", b =>
                {
                    b.Property<int>("EmailId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("ContactId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("EmailId");

                    b.HasIndex("ContactId");

                    b.ToTable("Emails");
                });

            modelBuilder.Entity("Business.Model.EnumHoldingRegistration", b =>
                {
                    b.Property<int>("EnumHoldingRegistrationId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("InternalName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("EnumHoldingRegistrationId");

                    b.ToTable("EnumHoldingRegistrations");

                    b.HasData(
                        new
                        {
                            EnumHoldingRegistrationId = 1,
                            Description = "Cows",
                            InternalName = "cow"
                        },
                        new
                        {
                            EnumHoldingRegistrationId = 2,
                            Description = "Fish",
                            InternalName = "fish"
                        },
                        new
                        {
                            EnumHoldingRegistrationId = 3,
                            Description = "Pigs",
                            InternalName = "pig"
                        },
                        new
                        {
                            EnumHoldingRegistrationId = 4,
                            Description = "Poultry",
                            InternalName = "poultry"
                        },
                        new
                        {
                            EnumHoldingRegistrationId = 5,
                            Description = "Sheep and Goats",
                            InternalName = "sheep_goats"
                        });
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
                        },
                        new
                        {
                            EnumRolePermissionId = 8,
                            Description = "Delete Contacts",
                            InternalName = "delete_contacts"
                        },
                        new
                        {
                            EnumRolePermissionId = 9,
                            Description = "View Holdings",
                            InternalName = "view_holdings"
                        },
                        new
                        {
                            EnumRolePermissionId = 19,
                            Description = "Edit Holdings",
                            InternalName = "edit_holdings"
                        });
                });

            modelBuilder.Entity("Business.Model.Holding", b =>
                {
                    b.Property<int>("HoldingId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<string>("GridReference")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("HoldingNumber")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("OwnerContactId");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasMaxLength(15);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("HoldingId");

                    b.HasIndex("OwnerContactId");

                    b.ToTable("Holdings");
                });

            modelBuilder.Entity("Business.Model.MapContactRelationship", b =>
                {
                    b.Property<int>("MapContactRelationshipId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContactOneId");

                    b.Property<int>("ContactTwoId");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("MapContactRelationshipId");

                    b.HasIndex("ContactOneId");

                    b.HasIndex("ContactTwoId");

                    b.HasIndex("ContactOneId", "ContactTwoId")
                        .IsUnique();

                    b.ToTable("MapContactRelationships");
                });

            modelBuilder.Entity("Business.Model.MapHoldingRegistrationToHolding", b =>
                {
                    b.Property<int>("MapHoldingRegistrationToHoldingId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HerdNumber")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<int>("HoldingId");

                    b.Property<int>("HoldingRegistrationId");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("MapHoldingRegistrationToHoldingId");

                    b.HasIndex("HoldingId");

                    b.HasIndex("HoldingRegistrationId");

                    b.HasIndex("HoldingId", "HoldingRegistrationId")
                        .IsUnique();

                    b.ToTable("MapHoldingRegistrationToHoldings");
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

            modelBuilder.Entity("Business.Model.Species", b =>
                {
                    b.Property<int>("SpeciesId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CharacteristicListId");

                    b.Property<bool>("IsPoultry");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("SpeciesId");

                    b.HasIndex("CharacteristicListId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Species");
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

            modelBuilder.Entity("Business.Model.ActionAgainstContactInfo", b =>
                {
                    b.HasOne("Business.Model.Contact", "ContactAffected")
                        .WithMany()
                        .HasForeignKey("ContactAffectedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Business.Model.User", "UserResponsible")
                        .WithMany()
                        .HasForeignKey("UserResponsibleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Business.Model.AnimalCharacteristic", b =>
                {
                    b.HasOne("Business.Model.AnimalCharacteristicList", "List")
                        .WithMany("Characteristics")
                        .HasForeignKey("ListId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Business.Model.Breed", b =>
                {
                    b.HasOne("Business.Model.Contact", "BreedSociety")
                        .WithMany()
                        .HasForeignKey("BreedSocietyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Business.Model.AnimalCharacteristicList", "CharacteristicList")
                        .WithMany()
                        .HasForeignKey("CharacteristicListId");

                    b.HasOne("Business.Model.Species", "SpeciesType")
                        .WithMany("Breeds")
                        .HasForeignKey("SpeciesId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Business.Model.Email", b =>
                {
                    b.HasOne("Business.Model.Contact", "Contact")
                        .WithMany("EmailAddresses")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Business.Model.Holding", b =>
                {
                    b.HasOne("Business.Model.Contact", "OwnerContact")
                        .WithMany()
                        .HasForeignKey("OwnerContactId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Business.Model.MapContactRelationship", b =>
                {
                    b.HasOne("Business.Model.Contact", "ContactOne")
                        .WithMany()
                        .HasForeignKey("ContactOneId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Business.Model.Contact", "ContactTwo")
                        .WithMany()
                        .HasForeignKey("ContactTwoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Business.Model.MapHoldingRegistrationToHolding", b =>
                {
                    b.HasOne("Business.Model.Holding", "Holding")
                        .WithMany("Registrations")
                        .HasForeignKey("HoldingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Business.Model.EnumHoldingRegistration", "HoldingRegistration")
                        .WithMany()
                        .HasForeignKey("HoldingRegistrationId")
                        .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity("Business.Model.Species", b =>
                {
                    b.HasOne("Business.Model.AnimalCharacteristicList", "CharacteristicList")
                        .WithMany()
                        .HasForeignKey("CharacteristicListId")
                        .OnDelete(DeleteBehavior.Cascade);
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
