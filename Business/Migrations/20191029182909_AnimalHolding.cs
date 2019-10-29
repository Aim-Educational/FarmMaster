using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class AnimalHolding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HoldingId",
                table: "Animals",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Animals_HoldingId",
                table: "Animals",
                column: "HoldingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Holdings_HoldingId",
                table: "Animals",
                column: "HoldingId",
                principalTable: "Holdings",
                principalColumn: "HoldingId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Holdings_HoldingId",
                table: "Animals");

            migrationBuilder.DropIndex(
                name: "IX_Animals_HoldingId",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "HoldingId",
                table: "Animals");
        }
    }
}
