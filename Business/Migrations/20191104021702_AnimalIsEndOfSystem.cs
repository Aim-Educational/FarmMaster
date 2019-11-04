using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class AnimalIsEndOfSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LifeEvents",
                columns: new[] { "LifeEventId", "Description", "Flags", "Name", "Target" },
                values: new object[] { 1000, "The animal was archived by a user.", 3, "Archived", 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1000);
        }
    }
}
