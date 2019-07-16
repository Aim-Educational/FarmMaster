using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class ContactAnonymise : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "Contacts",
                nullable: true,
                defaultValue: false);

            migrationBuilder.Sql(
                "UPDATE \"Contacts\" SET \"IsAnonymous\" = false;"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "IsAnonymous",
                table: "Contacts",
                nullable: false);

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 8, "Delete Contacts", "delete_contacts" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "Contacts");
        }
    }
}
