using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class PreventCascadeDeleteOnAnimals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AnimalCharacteristicLists_CharacteristicsId",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Contacts_OwnerId",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Species_SpeciesId",
                table: "Animals");

            migrationBuilder.AlterColumn<int>(
                name: "SpeciesId",
                table: "Animals",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Animals",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CharacteristicsId",
                table: "Animals",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AnimalCharacteristicLists_CharacteristicsId",
                table: "Animals",
                column: "CharacteristicsId",
                principalTable: "AnimalCharacteristicLists",
                principalColumn: "AnimalCharacteristicListId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Contacts_OwnerId",
                table: "Animals",
                column: "OwnerId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Species_SpeciesId",
                table: "Animals",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "SpeciesId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AnimalCharacteristicLists_CharacteristicsId",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Contacts_OwnerId",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Species_SpeciesId",
                table: "Animals");

            migrationBuilder.AlterColumn<int>(
                name: "SpeciesId",
                table: "Animals",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Animals",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CharacteristicsId",
                table: "Animals",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AnimalCharacteristicLists_CharacteristicsId",
                table: "Animals",
                column: "CharacteristicsId",
                principalTable: "AnimalCharacteristicLists",
                principalColumn: "AnimalCharacteristicListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Contacts_OwnerId",
                table: "Animals",
                column: "OwnerId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Species_SpeciesId",
                table: "Animals",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "SpeciesId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
