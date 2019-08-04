using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class AnimalCharacteristics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalCharacteristic_AnimalCharacteristicList_ListId",
                table: "AnimalCharacteristic");

            migrationBuilder.DropForeignKey(
                name: "FK_Breeds_AnimalCharacteristicList_CharacteristicListId",
                table: "Breeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_AnimalCharacteristicList_CharacteristicListId",
                table: "Species");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimalCharacteristicList",
                table: "AnimalCharacteristicList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimalCharacteristic",
                table: "AnimalCharacteristic");

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 19);

            migrationBuilder.DropColumn(
                name: "CalculatedType",
                table: "AnimalCharacteristic");

            migrationBuilder.RenameTable(
                name: "AnimalCharacteristicList",
                newName: "AnimalCharacteristicLists");

            migrationBuilder.RenameTable(
                name: "AnimalCharacteristic",
                newName: "AnimalCharacteristics");

            migrationBuilder.RenameIndex(
                name: "IX_AnimalCharacteristic_ListId",
                table: "AnimalCharacteristics",
                newName: "IX_AnimalCharacteristics_ListId");

            migrationBuilder.AddColumn<string>(
                name: "DataType",
                table: "AnimalCharacteristics",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimalCharacteristicLists",
                table: "AnimalCharacteristicLists",
                column: "AnimalCharacteristicListId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimalCharacteristics",
                table: "AnimalCharacteristics",
                column: "AnimalCharacteristicId");

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 10, "Edit Holdings", "edit_holdings" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 11, "View Species/Breeds", "view_species_breeds" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 12, "Edit Species/Breeds", "edit_species_breeds" });

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalCharacteristics_AnimalCharacteristicLists_ListId",
                table: "AnimalCharacteristics",
                column: "ListId",
                principalTable: "AnimalCharacteristicLists",
                principalColumn: "AnimalCharacteristicListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Breeds_AnimalCharacteristicLists_CharacteristicListId",
                table: "Breeds",
                column: "CharacteristicListId",
                principalTable: "AnimalCharacteristicLists",
                principalColumn: "AnimalCharacteristicListId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_AnimalCharacteristicLists_CharacteristicListId",
                table: "Species",
                column: "CharacteristicListId",
                principalTable: "AnimalCharacteristicLists",
                principalColumn: "AnimalCharacteristicListId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalCharacteristics_AnimalCharacteristicLists_ListId",
                table: "AnimalCharacteristics");

            migrationBuilder.DropForeignKey(
                name: "FK_Breeds_AnimalCharacteristicLists_CharacteristicListId",
                table: "Breeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_AnimalCharacteristicLists_CharacteristicListId",
                table: "Species");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimalCharacteristics",
                table: "AnimalCharacteristics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimalCharacteristicLists",
                table: "AnimalCharacteristicLists");

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 12);

            migrationBuilder.DropColumn(
                name: "DataType",
                table: "AnimalCharacteristics");

            migrationBuilder.RenameTable(
                name: "AnimalCharacteristics",
                newName: "AnimalCharacteristic");

            migrationBuilder.RenameTable(
                name: "AnimalCharacteristicLists",
                newName: "AnimalCharacteristicList");

            migrationBuilder.RenameIndex(
                name: "IX_AnimalCharacteristics_ListId",
                table: "AnimalCharacteristic",
                newName: "IX_AnimalCharacteristic_ListId");

            migrationBuilder.AddColumn<string>(
                name: "CalculatedType",
                table: "AnimalCharacteristic",
                nullable: false,
                computedColumnSql: "\"CalculatedType\"::json->'__TYPE'");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimalCharacteristic",
                table: "AnimalCharacteristic",
                column: "AnimalCharacteristicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimalCharacteristicList",
                table: "AnimalCharacteristicList",
                column: "AnimalCharacteristicListId");

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 19, "Edit Holdings", "edit_holdings" });

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalCharacteristic_AnimalCharacteristicList_ListId",
                table: "AnimalCharacteristic",
                column: "ListId",
                principalTable: "AnimalCharacteristicList",
                principalColumn: "AnimalCharacteristicListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Breeds_AnimalCharacteristicList_CharacteristicListId",
                table: "Breeds",
                column: "CharacteristicListId",
                principalTable: "AnimalCharacteristicList",
                principalColumn: "AnimalCharacteristicListId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_AnimalCharacteristicList_CharacteristicListId",
                table: "Species",
                column: "CharacteristicListId",
                principalTable: "AnimalCharacteristicList",
                principalColumn: "AnimalCharacteristicListId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
