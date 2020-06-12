using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class CascadeFixesAttempt2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Breeds_NoteOwners_NoteOwnerId",
                table: "Breeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_LocationHoldingInfo_HoldingId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_NoteOwners_NoteOwnerId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_NoteOwners_NoteOwnerId",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Locations_HoldingId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "HoldingId",
                table: "Locations");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "LocationHoldingInfo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocationId1",
                table: "LocationHoldingInfo",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationHoldingInfo_LocationId",
                table: "LocationHoldingInfo",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationHoldingInfo_LocationId1",
                table: "LocationHoldingInfo",
                column: "LocationId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Breeds_NoteOwners_NoteOwnerId",
                table: "Breeds",
                column: "NoteOwnerId",
                principalTable: "NoteOwners",
                principalColumn: "NoteOwnerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationHoldingInfo_Locations_LocationId",
                table: "LocationHoldingInfo",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationHoldingInfo_Locations_LocationId1",
                table: "LocationHoldingInfo",
                column: "LocationId1",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_NoteOwners_NoteOwnerId",
                table: "Locations",
                column: "NoteOwnerId",
                principalTable: "NoteOwners",
                principalColumn: "NoteOwnerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_NoteOwners_NoteOwnerId",
                table: "Species",
                column: "NoteOwnerId",
                principalTable: "NoteOwners",
                principalColumn: "NoteOwnerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Breeds_NoteOwners_NoteOwnerId",
                table: "Breeds");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationHoldingInfo_Locations_LocationId",
                table: "LocationHoldingInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationHoldingInfo_Locations_LocationId1",
                table: "LocationHoldingInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_NoteOwners_NoteOwnerId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_NoteOwners_NoteOwnerId",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_LocationHoldingInfo_LocationId",
                table: "LocationHoldingInfo");

            migrationBuilder.DropIndex(
                name: "IX_LocationHoldingInfo_LocationId1",
                table: "LocationHoldingInfo");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "LocationHoldingInfo");

            migrationBuilder.DropColumn(
                name: "LocationId1",
                table: "LocationHoldingInfo");

            migrationBuilder.AddColumn<int>(
                name: "HoldingId",
                table: "Locations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_HoldingId",
                table: "Locations",
                column: "HoldingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Breeds_NoteOwners_NoteOwnerId",
                table: "Breeds",
                column: "NoteOwnerId",
                principalTable: "NoteOwners",
                principalColumn: "NoteOwnerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_LocationHoldingInfo_HoldingId",
                table: "Locations",
                column: "HoldingId",
                principalTable: "LocationHoldingInfo",
                principalColumn: "LocationHoldingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_NoteOwners_NoteOwnerId",
                table: "Locations",
                column: "NoteOwnerId",
                principalTable: "NoteOwners",
                principalColumn: "NoteOwnerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_NoteOwners_NoteOwnerId",
                table: "Species",
                column: "NoteOwnerId",
                principalTable: "NoteOwners",
                principalColumn: "NoteOwnerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
