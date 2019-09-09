using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class LifeEventEntryDateTimeCreatedON : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateTimeCreated",
                table: "LifeEventEntries",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 15, "Edit Life Event Entries", "create_life_event_entry" });

            migrationBuilder.CreateIndex(
                name: "IX_LifeEventEntries_DateTimeCreated",
                table: "LifeEventEntries",
                column: "DateTimeCreated");

            migrationBuilder.CreateIndex(
                name: "IX_LifeEventEntries_DateTimeCreated_LifeEventId",
                table: "LifeEventEntries",
                columns: new[] { "DateTimeCreated", "LifeEventId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LifeEventEntries_DateTimeCreated",
                table: "LifeEventEntries");

            migrationBuilder.DropIndex(
                name: "IX_LifeEventEntries_DateTimeCreated_LifeEventId",
                table: "LifeEventEntries");

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 15);

            migrationBuilder.DropColumn(
                name: "DateTimeCreated",
                table: "LifeEventEntries");
        }
    }
}
