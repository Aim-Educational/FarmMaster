using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class Derp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1073741823,
                column: "Target",
                value: 1);

            migrationBuilder.UpdateData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1073741824,
                column: "Target",
                value: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1073741823,
                column: "Target",
                value: 0);

            migrationBuilder.UpdateData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1073741824,
                column: "Target",
                value: 0);
        }
    }
}
