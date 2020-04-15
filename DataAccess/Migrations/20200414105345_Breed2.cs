using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class Breed2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Breeds_NoteOwners_NoteOwnerId",
                table: "Breeds");

            migrationBuilder.AddForeignKey(
                name: "FK_Breeds_NoteOwners_NoteOwnerId",
                table: "Breeds",
                column: "NoteOwnerId",
                principalTable: "NoteOwners",
                principalColumn: "NoteOwnerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Breeds_NoteOwners_NoteOwnerId",
                table: "Breeds");

            migrationBuilder.AddForeignKey(
                name: "FK_Breeds_NoteOwners_NoteOwnerId",
                table: "Breeds",
                column: "NoteOwnerId",
                principalTable: "NoteOwners",
                principalColumn: "NoteOwnerId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
