using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataAccess.Migrations
{
    public partial class LocationsInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationHoldingInfo",
                columns: table => new
                {
                    LocationHoldingId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HoldingNumber = table.Column<string>(maxLength: 150, nullable: false),
                    GridReference = table.Column<string>(maxLength: 150, nullable: false),
                    Address = table.Column<string>(maxLength: 150, nullable: false),
                    Postcode = table.Column<string>(maxLength: 150, nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationHoldingInfo", x => x.LocationHoldingId);
                    table.ForeignKey(
                        name: "FK_LocationHoldingInfo_Contacts_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(maxLength: 150, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    HoldingId = table.Column<int>(nullable: true),
                    NoteOwnerId = table.Column<int>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Locations_LocationHoldingInfo_HoldingId",
                        column: x => x.HoldingId,
                        principalTable: "LocationHoldingInfo",
                        principalColumn: "LocationHoldingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Locations_NoteOwners_NoteOwnerId",
                        column: x => x.NoteOwnerId,
                        principalTable: "NoteOwners",
                        principalColumn: "NoteOwnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationHoldingInfo_OwnerId",
                table: "LocationHoldingInfo",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_HoldingId",
                table: "Locations",
                column: "HoldingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_NoteOwnerId",
                table: "Locations",
                column: "NoteOwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "LocationHoldingInfo");
        }
    }
}
