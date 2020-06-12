﻿// <auto-generated />
using System;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataAccess.Migrations
{
    [DbContext(typeof(FarmMasterContext))]
    partial class FarmMasterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("DataAccess.Breed", b =>
                {
                    b.Property<int>("BreedId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("character varying(75)")
                        .HasMaxLength(75);

                    b.Property<int?>("NoteOwnerId")
                        .HasColumnType("integer");

                    b.Property<int?>("SpeciesId")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.HasKey("BreedId");

                    b.HasIndex("NoteOwnerId");

                    b.HasIndex("SpeciesId");

                    b.ToTable("Breeds");

                    b.HasData(
                        new
                        {
                            BreedId = 2147383648,
                            Name = "English Longhorn",
                            SpeciesId = 1
                        },
                        new
                        {
                            BreedId = 2147383649,
                            Name = "Red Poll",
                            SpeciesId = 1
                        },
                        new
                        {
                            BreedId = 2147383650,
                            Name = "White Park",
                            SpeciesId = 1
                        },
                        new
                        {
                            BreedId = 2147383651,
                            Name = "Hereford",
                            SpeciesId = 1
                        },
                        new
                        {
                            BreedId = 2147383652,
                            Name = "Highland",
                            SpeciesId = 1
                        },
                        new
                        {
                            BreedId = 2147383653,
                            Name = "Aryshire",
                            SpeciesId = 1
                        },
                        new
                        {
                            BreedId = 2147383654,
                            Name = "Aberdeen Angus",
                            SpeciesId = 1
                        },
                        new
                        {
                            BreedId = 2147383655,
                            Name = "South Devon",
                            SpeciesId = 1
                        },
                        new
                        {
                            BreedId = 2147383656,
                            Name = "British White",
                            SpeciesId = 1
                        },
                        new
                        {
                            BreedId = 2147383657,
                            Name = "Belted Galloway",
                            SpeciesId = 1
                        });
                });

            modelBuilder.Entity("DataAccess.Contact", b =>
                {
                    b.Property<int>("ContactId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Email")
                        .HasColumnType("character varying(75)")
                        .HasMaxLength(75);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(75)")
                        .HasMaxLength(75);

                    b.Property<int?>("NoteOwnerId")
                        .HasColumnType("integer");

                    b.Property<string>("Phone")
                        .HasColumnType("character varying(75)")
                        .HasMaxLength(75);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("ContactId");

                    b.HasIndex("NoteOwnerId");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("DataAccess.Location", b =>
                {
                    b.Property<int>("LocationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(150)")
                        .HasMaxLength(150);

                    b.Property<int?>("NoteOwnerId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("LocationId");

                    b.HasIndex("NoteOwnerId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("DataAccess.LocationHolding", b =>
                {
                    b.Property<int>("LocationHoldingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("character varying(150)")
                        .HasMaxLength(150);

                    b.Property<string>("GridReference")
                        .IsRequired()
                        .HasColumnType("character varying(150)")
                        .HasMaxLength(150);

                    b.Property<string>("HoldingNumber")
                        .IsRequired()
                        .HasColumnType("character varying(150)")
                        .HasMaxLength(150);

                    b.Property<int>("LocationId")
                        .HasColumnType("integer");

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("character varying(150)")
                        .HasMaxLength(150);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.HasKey("LocationHoldingId");

                    b.HasIndex("LocationId")
                        .IsUnique();

                    b.HasIndex("OwnerId");

                    b.ToTable("LocationHoldingInfo");
                });

            modelBuilder.Entity("DataAccess.LogEntry", b =>
                {
                    b.Property<int>("LogEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("character varying(75)")
                        .HasMaxLength(75);

                    b.Property<DateTimeOffset>("DateLogged")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EventId")
                        .HasColumnType("integer");

                    b.Property<string>("EventName")
                        .HasColumnType("character varying(75)")
                        .HasMaxLength(75);

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("character varying(512)")
                        .HasMaxLength(512);

                    b.Property<string>("StateJson")
                        .HasColumnType("jsonb")
                        .HasMaxLength(2048);

                    b.HasKey("LogEntryId");

                    b.ToTable("LogEntries");
                });

            modelBuilder.Entity("DataAccess.NoteEntry", b =>
                {
                    b.Property<int>("NoteEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("character varying(75)")
                        .HasMaxLength(75);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<int>("NoteOwnerId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.HasKey("NoteEntryId");

                    b.HasIndex("NoteOwnerId");

                    b.ToTable("NoteEntries");
                });

            modelBuilder.Entity("DataAccess.NoteOwner", b =>
                {
                    b.Property<int>("NoteOwnerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.HasKey("NoteOwnerId");

                    b.ToTable("NoteOwners");
                });

            modelBuilder.Entity("DataAccess.Settings", b =>
                {
                    b.Property<int>("SettingsKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("SmtpPassword")
                        .HasColumnType("text");

                    b.Property<int>("SmtpPort")
                        .HasColumnType("integer");

                    b.Property<string>("SmtpServer")
                        .HasColumnType("text");

                    b.Property<string>("SmtpUsername")
                        .HasColumnType("text");

                    b.HasKey("SettingsKey");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("DataAccess.Species", b =>
                {
                    b.Property<int>("SpeciesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<TimeSpan>("GestrationPeriod")
                        .HasColumnType("interval");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(75)")
                        .HasMaxLength(75);

                    b.Property<int?>("NoteOwnerId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.HasKey("SpeciesId");

                    b.HasIndex("NoteOwnerId");

                    b.ToTable("Species");

                    b.HasData(
                        new
                        {
                            SpeciesId = 1,
                            GestrationPeriod = new TimeSpan(283, 0, 0, 0, 0),
                            Name = "Cow"
                        });
                });

            modelBuilder.Entity("DataAccess.Breed", b =>
                {
                    b.HasOne("DataAccess.NoteOwner", "NoteOwner")
                        .WithMany()
                        .HasForeignKey("NoteOwnerId");

                    b.HasOne("DataAccess.Species", "Species")
                        .WithMany("Breeds")
                        .HasForeignKey("SpeciesId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DataAccess.Contact", b =>
                {
                    b.HasOne("DataAccess.NoteOwner", "NoteOwner")
                        .WithMany()
                        .HasForeignKey("NoteOwnerId");
                });

            modelBuilder.Entity("DataAccess.Location", b =>
                {
                    b.HasOne("DataAccess.NoteOwner", "NoteOwner")
                        .WithMany()
                        .HasForeignKey("NoteOwnerId");
                });

            modelBuilder.Entity("DataAccess.LocationHolding", b =>
                {
                    b.HasOne("DataAccess.Location", "Location")
                        .WithOne("Holding")
                        .HasForeignKey("DataAccess.LocationHolding", "LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccess.Contact", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DataAccess.NoteEntry", b =>
                {
                    b.HasOne("DataAccess.NoteOwner", "NoteOwner")
                        .WithMany("NoteEntries")
                        .HasForeignKey("NoteOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataAccess.Species", b =>
                {
                    b.HasOne("DataAccess.NoteOwner", "NoteOwner")
                        .WithMany()
                        .HasForeignKey("NoteOwnerId");
                });
#pragma warning restore 612, 618
        }
    }
}
