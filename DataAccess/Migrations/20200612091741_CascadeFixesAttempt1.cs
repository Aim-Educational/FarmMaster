using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class CascadeFixesAttempt1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Species_NoteOwners_NoteOwnerId",
                table: "Species");

            migrationBuilder.AddForeignKey(
                name: "FK_Species_NoteOwners_NoteOwnerId",
                table: "Species",
                column: "NoteOwnerId",
                principalTable: "NoteOwners",
                principalColumn: "NoteOwnerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Species_NoteOwners_NoteOwnerId",
                table: "Species");

            migrationBuilder.AddForeignKey(
                name: "FK_Species_NoteOwners_NoteOwnerId",
                table: "Species",
                column: "NoteOwnerId",
                principalTable: "NoteOwners",
                principalColumn: "NoteOwnerId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
