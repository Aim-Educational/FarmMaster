using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class CascadeFixesAttempt3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationHoldingInfo_Locations_LocationId1",
                table: "LocationHoldingInfo");

            migrationBuilder.DropIndex(
                name: "IX_LocationHoldingInfo_LocationId1",
                table: "LocationHoldingInfo");

            migrationBuilder.DropColumn(
                name: "LocationId1",
                table: "LocationHoldingInfo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId1",
                table: "LocationHoldingInfo",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationHoldingInfo_LocationId1",
                table: "LocationHoldingInfo",
                column: "LocationId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationHoldingInfo_Locations_LocationId1",
                table: "LocationHoldingInfo",
                column: "LocationId1",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
