using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class LogEntry3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Event",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "LogEntries");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "LogEntries",
                maxLength: 75,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "LogEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "LogEntries",
                maxLength: 75,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "LogEntries",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "EventName",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "LogEntries");

            migrationBuilder.AddColumn<int>(
                name: "Event",
                table: "LogEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "LogEntries",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }
    }
}
