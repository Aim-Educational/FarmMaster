using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    public partial class ContactTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactTokens",
                columns: table => new
                {
                    ContactTokenId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Token = table.Column<string>(maxLength: 36, nullable: false),
                    UsageType = table.Column<string>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    Expiry = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactTokens", x => x.ContactTokenId);
                    table.ForeignKey(
                        name: "FK_ContactTokens_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactTokens_ContactId",
                table: "ContactTokens",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactTokens_Token",
                table: "ContactTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactTokens_UsageType",
                table: "ContactTokens",
                column: "UsageType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactTokens");
        }
    }
}
