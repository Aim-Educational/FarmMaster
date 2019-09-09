using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    public partial class TooMuchStuff1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    AnimalId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 75, nullable: false),
                    Sex = table.Column<int>(nullable: false),
                    Tag = table.Column<string>(maxLength: 20, nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    MumId = table.Column<int>(nullable: true),
                    DadId = table.Column<int>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.AnimalId);
                    table.ForeignKey(
                        name: "FK_Animals_Animals_DadId",
                        column: x => x.DadId,
                        principalTable: "Animals",
                        principalColumn: "AnimalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Animals_Animals_MumId",
                        column: x => x.MumId,
                        principalTable: "Animals",
                        principalColumn: "AnimalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Animals_Contacts_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MapBreedToAnimals",
                columns: table => new
                {
                    MapBreedToAnimalId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    BreedId = table.Column<int>(nullable: false),
                    AnimalId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapBreedToAnimals", x => x.MapBreedToAnimalId);
                    table.ForeignKey(
                        name: "FK_MapBreedToAnimals_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "AnimalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapBreedToAnimals_Breeds_BreedId",
                        column: x => x.BreedId,
                        principalTable: "Breeds",
                        principalColumn: "BreedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MapLifeEventEntryToAnimals",
                columns: table => new
                {
                    MapLifeEventEntryToAnimalId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AnimalId = table.Column<int>(nullable: false),
                    LifeEventEntryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapLifeEventEntryToAnimals", x => x.MapLifeEventEntryToAnimalId);
                    table.ForeignKey(
                        name: "FK_MapLifeEventEntryToAnimals_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "AnimalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapLifeEventEntryToAnimals_LifeEventEntries_LifeEventEntryId",
                        column: x => x.LifeEventEntryId,
                        principalTable: "LifeEventEntries",
                        principalColumn: "LifeEventEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animals_DadId",
                table: "Animals",
                column: "DadId");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_MumId",
                table: "Animals",
                column: "MumId");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_OwnerId",
                table: "Animals",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MapBreedToAnimals_AnimalId",
                table: "MapBreedToAnimals",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_MapBreedToAnimals_BreedId",
                table: "MapBreedToAnimals",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_MapBreedToAnimals_AnimalId_BreedId",
                table: "MapBreedToAnimals",
                columns: new[] { "AnimalId", "BreedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MapLifeEventEntryToAnimals_AnimalId",
                table: "MapLifeEventEntryToAnimals",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLifeEventEntryToAnimals_LifeEventEntryId",
                table: "MapLifeEventEntryToAnimals",
                column: "LifeEventEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLifeEventEntryToAnimals_AnimalId_LifeEventEntryId",
                table: "MapLifeEventEntryToAnimals",
                columns: new[] { "AnimalId", "LifeEventEntryId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapBreedToAnimals");

            migrationBuilder.DropTable(
                name: "MapLifeEventEntryToAnimals");

            migrationBuilder.DropTable(
                name: "Animals");
        }
    }
}
