using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class SpeciesNullableNoteOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Species_NoteOwners_NoteOwnerId",
                table: "Species");

            migrationBuilder.AlterColumn<int>(
                name: "NoteOwnerId",
                table: "Species",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

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
                name: "FK_Species_NoteOwners_NoteOwnerId",
                table: "Species");

            migrationBuilder.AlterColumn<int>(
                name: "NoteOwnerId",
                table: "Species",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

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
