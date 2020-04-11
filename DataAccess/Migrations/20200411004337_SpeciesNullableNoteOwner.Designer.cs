﻿// <auto-generated />
using System;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataAccess.Migrations
{
    [DbContext(typeof(FarmMasterContext))]
    [Migration("20200411004337_SpeciesNullableNoteOwner")]
    partial class SpeciesNullableNoteOwner
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

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
                });

            modelBuilder.Entity("DataAccess.Contact", b =>
                {
                    b.HasOne("DataAccess.NoteOwner", "NoteOwner")
                        .WithMany()
                        .HasForeignKey("NoteOwnerId");
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
