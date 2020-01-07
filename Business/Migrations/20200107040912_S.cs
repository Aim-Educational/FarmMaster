using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    public partial class S : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimalGroupScripts",
                columns: table => new
                {
                    AnimalGroupScriptId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 75, nullable: true),
                    Bytecode = table.Column<byte[]>(maxLength: 8192, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalGroupScripts", x => x.AnimalGroupScriptId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalGroupScripts_Name",
                table: "AnimalGroupScripts",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalGroupScripts");
        }
    }
}
