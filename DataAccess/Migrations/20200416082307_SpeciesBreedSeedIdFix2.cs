using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DataAccess.Migrations
{
    public partial class SpeciesBreedSeedIdFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Species",
                columns: new[] { "SpeciesId", "GestrationPeriod", "Name", "NoteOwnerId" },
                values: new object[] { 1, new TimeSpan(283, 0, 0, 0, 0), "Cow", null });

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
        }
    }
}
