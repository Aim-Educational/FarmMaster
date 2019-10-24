using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class AnimalSingleImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Animals",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Animals_ImageId",
                table: "Animals",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Images_ImageId",
                table: "Animals",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "ImageId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Images_ImageId",
                table: "Animals");

            migrationBuilder.DropIndex(
                name: "IX_Animals_ImageId",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Animals");
        }
    }
}
