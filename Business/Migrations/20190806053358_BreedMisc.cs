using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class BreedMisc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Breeds_AnimalCharacteristicLists_CharacteristicListId",
                table: "Breeds");

            migrationBuilder.AlterColumn<int>(
                name: "CharacteristicListId",
                table: "Breeds",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Breeds_Name_SpeciesId",
                table: "Breeds",
                columns: new[] { "Name", "SpeciesId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Breeds_AnimalCharacteristicLists_CharacteristicListId",
                table: "Breeds",
                column: "CharacteristicListId",
                principalTable: "AnimalCharacteristicLists",
                principalColumn: "AnimalCharacteristicListId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Breeds_AnimalCharacteristicLists_CharacteristicListId",
                table: "Breeds");

            migrationBuilder.DropIndex(
                name: "IX_Breeds_Name_SpeciesId",
                table: "Breeds");

            migrationBuilder.AlterColumn<int>(
                name: "CharacteristicListId",
                table: "Breeds",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Breeds_AnimalCharacteristicLists_CharacteristicListId",
                table: "Breeds",
                column: "CharacteristicListId",
                principalTable: "AnimalCharacteristicLists",
                principalColumn: "AnimalCharacteristicListId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
