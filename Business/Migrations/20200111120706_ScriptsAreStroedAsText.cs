using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class ScriptsAreStroedAsText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bytecode",
                table: "AnimalGroupScripts");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "AnimalGroupScripts",
                maxLength: 8192,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "AnimalGroupScripts");

            migrationBuilder.AddColumn<byte[]>(
                name: "Bytecode",
                table: "AnimalGroupScripts",
                maxLength: 8192,
                nullable: true);
        }
    }
}
