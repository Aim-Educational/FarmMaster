using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class RoleHierarchy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HierarchyOrder",
                table: "Roles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 7, "Assign Roles", "assign_roles" });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "HierarchyOrder",
                table: "Roles");
        }
    }
}
