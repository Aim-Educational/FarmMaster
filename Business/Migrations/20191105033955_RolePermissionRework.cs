using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class RolePermissionRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 1,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Assign roles", "assign_roles" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 2,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Create roles", "create_roles" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 3,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Delete roles", "delete_roles" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 4,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Edit roles", "edit_roles" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 5,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "View roles", "view_roles" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 6,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Create contacts", "create_contacts" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 7,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Delete contacts", "delete_contacts" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 8,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Edit contacts", "edit_contacts" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 9,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "View contacts", "view_contacts" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 10,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Create holdings", "create_holdings" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 11,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Delete holdings", "delete_holdings" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 12,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Edit holdings", "edit_holdings" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 13,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "View holdings", "view_holdings" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 14,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Create life events", "create_life_events" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 15,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Delete life events", "delete_life_events" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 16, "Edit life events", "edit_life_events" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 17, "View life events", "view_life_events" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 18, "Use life event entries", "use_life_event_entry" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 19, "Create species & breeds", "create_species_breeds" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 20, "Delete species & breeds", "delete_species_breeds" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 21, "Edit species & breeds", "edit_species_breeds" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 22, "View species & breeds", "view_species_breeds" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 23, "Create animals", "create_animals" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 24, "Delete animals", "delete_animals" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 25, "Edit animals", "edit_animals" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 26, "View animals", "view_animals" });

            migrationBuilder.UpdateData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1,
                column: "Flags",
                value: 6);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 26);

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 1,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Edit Contacts", "edit_contacts" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 2,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "View Contacts", "view_contacts" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 3,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Edit Roles", "edit_roles" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 4,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "View Roles", "view_roles" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 5,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Edit Users", "edit_users" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 6,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "View Users", "view_users" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 7,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Assign Roles", "assign_roles" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 8,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Delete Contacts", "delete_contacts" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 9,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "View Holdings", "view_holdings" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 10,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Edit Holdings", "edit_holdings" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 11,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "View Species/Breeds", "view_species_breeds" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 12,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Edit Species/Breeds", "edit_species_breeds" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 13,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "View Life Events", "view_life_events" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 14,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Edit Life Events", "edit_life_events" });

            migrationBuilder.UpdateData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 15,
                columns: new[] { "Description", "InternalName" },
                values: new object[] { "Edit Life Event Entries", "create_life_event_entry" });

            migrationBuilder.UpdateData(
                table: "LifeEvents",
                keyColumn: "LifeEventId",
                keyValue: 1,
                column: "Flags",
                value: 2);

            migrationBuilder.InsertData(
                table: "LifeEvents",
                columns: new[] { "LifeEventId", "Description", "Flags", "Name", "Target" },
                values: new object[] { 2, "The animal was archived by a user.", 1, "Archived", 1 });
        }
    }
}
