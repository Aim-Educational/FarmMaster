using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class SpeciesBreedSeedIdFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 11);

            migrationBuilder.InsertData(
                table: "Breeds",
                columns: new[] { "BreedId", "Name", "NoteOwnerId", "SpeciesId" },
                values: new object[,]
                {
                    { 2147383648, "English Longhorn", null, 1 },
                    { 2147383649, "Red Poll", null, 1 },
                    { 2147383650, "White Park", null, 1 },
                    { 2147383651, "Hereford", null, 1 },
                    { 2147383652, "Highland", null, 1 },
                    { 2147383653, "Aryshire", null, 1 },
                    { 2147383654, "Aberdeen Angus", null, 1 },
                    { 2147383655, "South Devon", null, 1 },
                    { 2147383656, "British White", null, 1 },
                    { 2147383657, "Belted Galloway", null, 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2147383648);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2147383649);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2147383650);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2147383651);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2147383652);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2147383653);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2147383654);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2147383655);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2147383656);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 2147383657);

            migrationBuilder.InsertData(
                table: "Breeds",
                columns: new[] { "BreedId", "Name", "NoteOwnerId", "SpeciesId" },
                values: new object[,]
                {
                    { 2, "English Longhorn", null, 1 },
                    { 3, "Red Poll", null, 1 },
                    { 4, "White Park", null, 1 },
                    { 5, "Hereford", null, 1 },
                    { 6, "Highland", null, 1 },
                    { 7, "Aryshire", null, 1 },
                    { 8, "Aberdeen Angus", null, 1 },
                    { 9, "South Devon", null, 1 },
                    { 10, "British White", null, 1 },
                    { 11, "Belted Galloway", null, 1 }
                });
        }
    }
}
