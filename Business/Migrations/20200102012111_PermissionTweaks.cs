using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class PermissionTweaks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 14,
                column: "Description",
                value: "Create Life Events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 15,
                column: "Description",
                value: "Delete Life Events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 16,
                column: "Description",
                value: "Edit Life Events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 17,
                column: "Description",
                value: "View Life Events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 18,
                column: "Description",
                value: "Create Life Event Entries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 14,
                column: "Description",
                value: "Create Life events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 15,
                column: "Description",
                value: "Delete Life events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 16,
                column: "Description",
                value: "Edit Life events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 17,
                column: "Description",
                value: "View Life events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 18,
                column: "Description",
                value: "Use Life event entries");
        }
    }
}
