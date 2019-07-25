using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class HoldingHerdNumberStringLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HerdNumber",
                table: "MapHoldingRegistrationToHoldings",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 9, "Delete Contacts", "view_holdings" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 19, "Edit Holdings", "edit_holdings" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 19);

            migrationBuilder.AlterColumn<string>(
                name: "HerdNumber",
                table: "MapHoldingRegistrationToHoldings",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 25);
        }
    }
}
