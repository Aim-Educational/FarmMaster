using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataAccess.Migrations
{
    public partial class Notes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NoteOwnerId",
                table: "Contacts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NoteOwners",
                columns: table => new
                {
                    NoteOwnerId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteOwners", x => x.NoteOwnerId);
                });

            migrationBuilder.CreateTable(
                name: "NoteEntries",
                columns: table => new
                {
                    NoteEntryId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Category = table.Column<string>(maxLength: 75, nullable: false),
                    Content = table.Column<string>(maxLength: 256, nullable: false),
                    NoteOwnerId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteEntries", x => x.NoteEntryId);
                    table.ForeignKey(
                        name: "FK_NoteEntries_NoteOwners_NoteOwnerId",
                        column: x => x.NoteOwnerId,
                        principalTable: "NoteOwners",
                        principalColumn: "NoteOwnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_NoteOwnerId",
                table: "Contacts",
                column: "NoteOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteEntries_NoteOwnerId",
                table: "NoteEntries",
                column: "NoteOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_NoteOwners_NoteOwnerId",
                table: "Contacts",
                column: "NoteOwnerId",
                principalTable: "NoteOwners",
                principalColumn: "NoteOwnerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_NoteOwners_NoteOwnerId",
                table: "Contacts");

            migrationBuilder.DropTable(
                name: "NoteEntries");

            migrationBuilder.DropTable(
                name: "NoteOwners");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_NoteOwnerId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "NoteOwnerId",
                table: "Contacts");
        }
    }
}
