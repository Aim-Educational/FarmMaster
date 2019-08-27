using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class LifeEventStuff2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LifeEvents",
                columns: new[] { "LifeEventId", "Description", "IsBuiltin", "Name" },
                values: new object[] { 1, "The animal was born.", true, "Born" });

            migrationBuilder.InsertData(
                table: "LifeEventDynamicFieldInfo",
                columns: new[] { "LifeEventDynamicFieldInfoId", "Description", "LifeEventId", "Name", "Type" },
                values: new object[] { 1, "When the animal was born.", 1, "Date", "DateTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LifeEventDynamicFieldInfo",
                keyColumn: "LifeEventDynamicFieldInfoId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1);
        }
    }
}
