using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    public partial class EmailUnsubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactUnsubscribeEntries",
                columns: table => new
                {
                    ContactUnsubscribeEntryId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Address = table.Column<string>(nullable: false),
                    ContactId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUnsubscribeEntries", x => x.ContactUnsubscribeEntryId);
                    table.ForeignKey(
                        name: "FK_ContactUnsubscribeEntries_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactUnsubscribeTokens",
                columns: table => new
                {
                    ContactUnsubscribeTokenId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Address = table.Column<string>(nullable: false),
                    Token = table.Column<string>(nullable: false),
                    ExpiresUtc = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUnsubscribeTokens", x => x.ContactUnsubscribeTokenId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactUnsubscribeEntries_ContactId",
                table: "ContactUnsubscribeEntries",
                column: "ContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactUnsubscribeEntries");

            migrationBuilder.DropTable(
                name: "ContactUnsubscribeTokens");
        }
    }
}
