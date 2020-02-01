using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    public partial class AutomatedScripts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimalGroupScriptAutoEntries",
                columns: table => new
                {
                    AnimalGroupScriptAutoEntryId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AnimalGroupId = table.Column<int>(nullable: false),
                    AnimalGroupScriptId = table.Column<int>(nullable: false),
                    Parameters = table.Column<string>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalGroupScriptAutoEntries", x => x.AnimalGroupScriptAutoEntryId);
                    table.ForeignKey(
                        name: "FK_AnimalGroupScriptAutoEntries_AnimalGroups_AnimalGroupId",
                        column: x => x.AnimalGroupId,
                        principalTable: "AnimalGroups",
                        principalColumn: "AnimalGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalGroupScriptAutoEntries_AnimalGroupScripts_AnimalGroup~",
                        column: x => x.AnimalGroupScriptId,
                        principalTable: "AnimalGroupScripts",
                        principalColumn: "AnimalGroupScriptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalGroupScriptAutoEntries_AnimalGroupId",
                table: "AnimalGroupScriptAutoEntries",
                column: "AnimalGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalGroupScriptAutoEntries_AnimalGroupScriptId",
                table: "AnimalGroupScriptAutoEntries",
                column: "AnimalGroupScriptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalGroupScriptAutoEntries");
        }
    }
}
