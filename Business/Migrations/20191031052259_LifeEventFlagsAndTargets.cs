using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class LifeEventFlagsAndTargets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBuiltin",
                table: "LifeEvents");

            migrationBuilder.AddColumn<int>(
                name: "Flags",
                table: "LifeEvents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Target",
                table: "LifeEvents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1,
                columns: new[] { "Flags", "Target" },
                values: new object[] { 2, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flags",
                table: "LifeEvents");

            migrationBuilder.DropColumn(
                name: "Target",
                table: "LifeEvents");

            migrationBuilder.AddColumn<bool>(
                name: "IsBuiltin",
                table: "LifeEvents",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1,
                column: "IsBuiltin",
                value: true);
        }
    }
}
