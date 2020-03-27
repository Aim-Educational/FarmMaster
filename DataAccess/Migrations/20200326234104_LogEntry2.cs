using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class LogEntry2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "LogEntries");

            migrationBuilder.RenameColumn(
                name: "DataJson",
                table: "LogEntries",
                newName: "StateJson");

            migrationBuilder.AddColumn<int>(
                name: "Event",
                table: "LogEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "LogEntries",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Event",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "LogEntries");

            migrationBuilder.RenameColumn(
                name: "StateJson",
                table: "LogEntries",
                newName: "DataJson");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "LogEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
