using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class SpeciesBreedSeed : Migration
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
                    { 1, "English Longhorn", null, 1 },
                    { 2, "Red Poll", null, 1 },
                    { 3, "White Park", null, 1 },
                    { 4, "Hereford", null, 1 },
                    { 5, "Highland", null, 1 },
                    { 6, "Aryshire", null, 1 },
                    { 7, "Aberdeen Angus", null, 1 },
                    { 8, "South Devon", null, 1 },
                    { 9, "British White", null, 1 },
                    { 10, "Belted Galloway", null, 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "BreedId",
                keyValue: 1);

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
                table: "Species",
                keyColumn: "SpeciesId",
                keyValue: 1);
        }
    }
}
