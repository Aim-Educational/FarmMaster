using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    public partial class Holdings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnumHoldingRegistrations",
                columns: table => new
                {
                    EnumHoldingRegistrationId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    InternalName = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnumHoldingRegistrations", x => x.EnumHoldingRegistrationId);
                });

            migrationBuilder.CreateTable(
                name: "Holdings",
                columns: table => new
                {
                    HoldingId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    HoldingNumber = table.Column<string>(maxLength: 50, nullable: false),
                    GridReference = table.Column<string>(maxLength: 50, nullable: false),
                    Address = table.Column<string>(maxLength: 150, nullable: false),
                    Postcode = table.Column<string>(maxLength: 15, nullable: false),
                    OwnerContactId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holdings", x => x.HoldingId);
                    table.ForeignKey(
                        name: "FK_Holdings_Contacts_OwnerContactId",
                        column: x => x.OwnerContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MapHoldingRegistrationToHoldings",
                columns: table => new
                {
                    MapHoldingRegistrationToHoldingId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    HoldingId = table.Column<int>(nullable: false),
                    HoldingRegistrationId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapHoldingRegistrationToHoldings", x => x.MapHoldingRegistrationToHoldingId);
                    table.ForeignKey(
                        name: "FK_MapHoldingRegistrationToHoldings_Holdings_HoldingId",
                        column: x => x.HoldingId,
                        principalTable: "Holdings",
                        principalColumn: "HoldingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapHoldingRegistrationToHoldings_EnumHoldingRegistrations_H~",
                        column: x => x.HoldingRegistrationId,
                        principalTable: "EnumHoldingRegistrations",
                        principalColumn: "EnumHoldingRegistrationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EnumHoldingRegistrations",
                columns: new[] { "EnumHoldingRegistrationId", "Description", "InternalName" },
                values: new object[,]
                {
                    { 1, "Cows", "cow" },
                    { 2, "Fish", "fish" },
                    { 3, "Pigs", "pig" },
                    { 4, "Poultry", "poultry" },
                    { 5, "Sheeps and Goats", "sheep_goats" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Holdings_OwnerContactId",
                table: "Holdings",
                column: "OwnerContactId");

            migrationBuilder.CreateIndex(
                name: "IX_MapHoldingRegistrationToHoldings_HoldingId",
                table: "MapHoldingRegistrationToHoldings",
                column: "HoldingId");

            migrationBuilder.CreateIndex(
                name: "IX_MapHoldingRegistrationToHoldings_HoldingRegistrationId",
                table: "MapHoldingRegistrationToHoldings",
                column: "HoldingRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_MapHoldingRegistrationToHoldings_HoldingId_HoldingRegistrat~",
                table: "MapHoldingRegistrationToHoldings",
                columns: new[] { "HoldingId", "HoldingRegistrationId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapHoldingRegistrationToHoldings");

            migrationBuilder.DropTable(
                name: "Holdings");

            migrationBuilder.DropTable(
                name: "EnumHoldingRegistrations");
        }
    }
}
