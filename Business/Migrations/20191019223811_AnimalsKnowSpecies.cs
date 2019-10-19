using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class AnimalsKnowSpecies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpeciesId",
                table: "Animals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Animals_SpeciesId",
                table: "Animals",
                column: "SpeciesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Species_SpeciesId",
                table: "Animals",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "SpeciesId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Species_SpeciesId",
                table: "Animals");

            migrationBuilder.DropIndex(
                name: "IX_Animals_SpeciesId",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "SpeciesId",
                table: "Animals");
        }
    }
}
