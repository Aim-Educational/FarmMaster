using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class SpeciesBreedSeedIdFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2,
                column: "Name",
                value: "English Longhorn");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 3,
                column: "Name",
                value: "Red Poll");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 4,
                column: "Name",
                value: "White Park");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 5,
                column: "Name",
                value: "Hereford");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 6,
                column: "Name",
                value: "Highland");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 7,
                column: "Name",
                value: "Aryshire");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 8,
                column: "Name",
                value: "Aberdeen Angus");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 9,
                column: "Name",
                value: "South Devon");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 10,
                column: "Name",
                value: "British White");

            migrationBuilder.InsertData(
                table: "Breeds",
                columns: new[] { "BreedId", "Name", "NoteOwnerId", "SpeciesId" },
                values: new object[] { 11, "Belted Galloway", null, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 11);

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2,
                column: "Name",
                value: "Red Poll");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 3,
                column: "Name",
                value: "White Park");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 4,
                column: "Name",
                value: "Hereford");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 5,
                column: "Name",
                value: "Highland");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 6,
                column: "Name",
                value: "Aryshire");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 7,
                column: "Name",
                value: "Aberdeen Angus");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 8,
                column: "Name",
                value: "South Devon");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 9,
                column: "Name",
                value: "British White");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 10,
                column: "Name",
                value: "Belted Galloway");

            migrationBuilder.InsertData(
                table: "Breeds",
                columns: new[] { "BreedId", "Name", "NoteOwnerId", "SpeciesId" },
                values: new object[] { 1, "English Longhorn", null, 1 });
        }
    }
}
