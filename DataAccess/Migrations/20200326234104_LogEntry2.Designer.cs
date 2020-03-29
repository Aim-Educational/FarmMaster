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
    [Migration("20200326234104_LogEntry2")]
    partial class LogEntry2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("DataAccess.LogEntry", b =>
                {
                    b.Property<int>("LogEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("DateLogged")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Event")
                        .HasColumnType("integer");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<string>("StateJson")
                        .HasColumnType("jsonb")
                        .HasMaxLength(2048);

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("character varying(512)")
                        .HasMaxLength(512);

                    b.HasKey("LogEntryId");

                    b.ToTable("LogEntries");
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
#pragma warning restore 612, 618
        }
    }
}