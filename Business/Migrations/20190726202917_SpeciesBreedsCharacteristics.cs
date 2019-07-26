using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    public partial class SpeciesBreedsCharacteristics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimalCharacteristicList",
                columns: table => new
                {
                    AnimalCharacteristicListId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalCharacteristicList", x => x.AnimalCharacteristicListId);
                });

            migrationBuilder.CreateTable(
                name: "AnimalCharacteristic",
                columns: table => new
                {
                    AnimalCharacteristicId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 75, nullable: false),
                    Data = table.Column<string>(maxLength: 65535, nullable: false),
                    ListId = table.Column<int>(nullable: false),
                    CalculatedType = table.Column<string>(nullable: false, computedColumnSql: "\"CalculatedType\"::json->'__TYPE'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalCharacteristic", x => x.AnimalCharacteristicId);
                    table.ForeignKey(
                        name: "FK_AnimalCharacteristic_AnimalCharacteristicList_ListId",
                        column: x => x.ListId,
                        principalTable: "AnimalCharacteristicList",
                        principalColumn: "AnimalCharacteristicListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    SpeciesId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    GestrationPeriod = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsPoultry = table.Column<bool>(nullable: false),
                    CharacteristicListId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.SpeciesId);
                    table.ForeignKey(
                        name: "FK_Species_AnimalCharacteristicList_CharacteristicListId",
                        column: x => x.CharacteristicListId,
                        principalTable: "AnimalCharacteristicList",
                        principalColumn: "AnimalCharacteristicListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Breeds",
                columns: table => new
                {
                    BreedId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 75, nullable: false),
                    IsRegisterable = table.Column<bool>(nullable: false),
                    BreedSocietyId = table.Column<int>(nullable: false),
                    SpeciesId = table.Column<int>(nullable: false),
                    CharacteristicListId = table.Column<int>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breeds", x => x.BreedId);
                    table.ForeignKey(
                        name: "FK_Breeds_Contacts_BreedSocietyId",
                        column: x => x.BreedSocietyId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Breeds_AnimalCharacteristicList_CharacteristicListId",
                        column: x => x.CharacteristicListId,
                        principalTable: "AnimalCharacteristicList",
                        principalColumn: "AnimalCharacteristicListId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Breeds_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 9,
                column: "Description",
                value: "View Holdings");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalCharacteristic_ListId",
                table: "AnimalCharacteristic",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_Breeds_BreedSocietyId",
                table: "Breeds",
                column: "BreedSocietyId");

            migrationBuilder.CreateIndex(
                name: "IX_Breeds_CharacteristicListId",
                table: "Breeds",
                column: "CharacteristicListId");

            migrationBuilder.CreateIndex(
                name: "IX_Breeds_Name",
                table: "Breeds",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Breeds_SpeciesId",
                table: "Breeds",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Species_CharacteristicListId",
                table: "Species",
                column: "CharacteristicListId");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Name",
                table: "Species",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalCharacteristic");

            migrationBuilder.DropTable(
                name: "Breeds");

            migrationBuilder.DropTable(
                name: "Species");

            migrationBuilder.DropTable(
                name: "AnimalCharacteristicList");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 9,
                column: "Description",
                value: "Delete Contacts");
        }
    }
}
