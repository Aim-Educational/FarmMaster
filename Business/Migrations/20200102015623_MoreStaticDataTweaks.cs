using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class MoreStaticDataTweaks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LifeEventDynamicFieldInfo",
                keyColumn: "LifeEventDynamicFieldInfoId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1000);

            migrationBuilder.DeleteData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "LifeEvents",
                columns: new[] { "LifeEventId", "Description", "Flags", "Name", "Target" },
                values: new object[] { 1073741823, "The animal was born.", 6, "Born", 0 });

            migrationBuilder.InsertData(
                table: "LifeEvents",
                columns: new[] { "LifeEventId", "Description", "Flags", "Name", "Target" },
                values: new object[] { 1073741824, "The animal was archived by a user.", 7, "Archived", 0 });

            migrationBuilder.InsertData(
                table: "LifeEventDynamicFieldInfo",
                columns: new[] { "LifeEventDynamicFieldInfoId", "Description", "LifeEventId", "Name", "Type" },
                values: new object[] { 1073741823, "When the animal was born", 1073741823, "Date", "DateTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LifeEventDynamicFieldInfo",
                keyColumn: "LifeEventDynamicFieldInfoId",
                keyValue: 1073741823);

            migrationBuilder.DeleteData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1073741824);

            migrationBuilder.DeleteData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1073741823);

            migrationBuilder.InsertData(
                table: "LifeEvents",
                columns: new[] { "LifeEventId", "Description", "Flags", "Name", "Target" },
                values: new object[] { 1, "The animal was born.", 6, "Born", 1 });

            migrationBuilder.InsertData(
                table: "LifeEvents",
                columns: new[] { "LifeEventId", "Description", "Flags", "Name", "Target" },
                values: new object[] { 1000, "The animal was archived by a user.", 7, "Archived", 1 });

            migrationBuilder.InsertData(
                table: "LifeEventDynamicFieldInfo",
                columns: new[] { "LifeEventDynamicFieldInfoId", "Description", "LifeEventId", "Name", "Type" },
                values: new object[] { 1, "When the animal was born.", 1, "Date", "DateTime" });
        }
    }
}
