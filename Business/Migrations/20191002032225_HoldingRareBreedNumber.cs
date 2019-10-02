using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class HoldingRareBreedNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RareBreedNumber",
                table: "MapHoldingRegistrationToHoldings",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CharacteristicsId",
                table: "Animals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Animals_CharacteristicsId",
                table: "Animals",
                column: "CharacteristicsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AnimalCharacteristicLists_CharacteristicsId",
                table: "Animals",
                column: "CharacteristicsId",
                principalTable: "AnimalCharacteristicLists",
                principalColumn: "AnimalCharacteristicListId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AnimalCharacteristicLists_CharacteristicsId",
                table: "Animals");

            migrationBuilder.DropIndex(
                name: "IX_Animals_CharacteristicsId",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "RareBreedNumber",
                table: "MapHoldingRegistrationToHoldings");

            migrationBuilder.DropColumn(
                name: "CharacteristicsId",
                table: "Animals");
        }
    }
}
