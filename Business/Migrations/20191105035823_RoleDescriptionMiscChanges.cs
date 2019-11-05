using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class RoleDescriptionMiscChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 1,
                column: "Description",
                value: "Assign Roles");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 2,
                column: "Description",
                value: "Create Roles");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 3,
                column: "Description",
                value: "Delete Roles");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 4,
                column: "Description",
                value: "Edit Roles");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 5,
                column: "Description",
                value: "View Roles");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 6,
                column: "Description",
                value: "Create Contacts");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 7,
                column: "Description",
                value: "Delete Contacts");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 8,
                column: "Description",
                value: "Edit Contacts");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 9,
                column: "Description",
                value: "View Contacts");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 10,
                column: "Description",
                value: "Create Holdings");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 11,
                column: "Description",
                value: "Delete Holdings");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 12,
                column: "Description",
                value: "Edit Holdings");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 13,
                column: "Description",
                value: "View Holdings");

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

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 19,
                column: "Description",
                value: "Create Species & Breeds");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 20,
                column: "Description",
                value: "Delete Species & Breeds");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 21,
                column: "Description",
                value: "Edit Species & Breeds");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 22,
                column: "Description",
                value: "View Species & Breeds");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 23,
                column: "Description",
                value: "Create Animals");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 24,
                column: "Description",
                value: "Delete Animals");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 25,
                column: "Description",
                value: "Edit Animals");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 26,
                column: "Description",
                value: "View Animals");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 1,
                column: "Description",
                value: "Assign roles");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 2,
                column: "Description",
                value: "Create roles");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 3,
                column: "Description",
                value: "Delete roles");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 4,
                column: "Description",
                value: "Edit roles");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 5,
                column: "Description",
                value: "View roles");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 6,
                column: "Description",
                value: "Create contacts");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 7,
                column: "Description",
                value: "Delete contacts");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 8,
                column: "Description",
                value: "Edit contacts");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 9,
                column: "Description",
                value: "View contacts");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 10,
                column: "Description",
                value: "Create holdings");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 11,
                column: "Description",
                value: "Delete holdings");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 12,
                column: "Description",
                value: "Edit holdings");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 13,
                column: "Description",
                value: "View holdings");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 14,
                column: "Description",
                value: "Create life events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 15,
                column: "Description",
                value: "Delete life events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 16,
                column: "Description",
                value: "Edit life events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 17,
                column: "Description",
                value: "View life events");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 18,
                column: "Description",
                value: "Use life event entries");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 19,
                column: "Description",
                value: "Create species & breeds");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 20,
                column: "Description",
                value: "Delete species & breeds");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 21,
                column: "Description",
                value: "Edit species & breeds");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 22,
                column: "Description",
                value: "View species & breeds");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 23,
                column: "Description",
                value: "Create animals");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 24,
                column: "Description",
                value: "Delete animals");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 25,
                column: "Description",
                value: "Edit animals");

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 26,
                column: "Description",
                value: "View animals");
        }
    }
}
