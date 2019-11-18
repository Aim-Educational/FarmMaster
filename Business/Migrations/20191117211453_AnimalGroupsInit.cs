using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    public partial class AnimalGroupsInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimalGroups",
                columns: table => new
                {
                    AnimalGroupId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 75, nullable: true),
                    Description = table.Column<string>(maxLength: 150, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalGroups", x => x.AnimalGroupId);
                });

            migrationBuilder.CreateTable(
                name: "MapAnimalToAnimalGroups",
                columns: table => new
                {
                    MapAnimalToAnimalGroupId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AnimalId = table.Column<int>(nullable: false),
                    AnimalGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapAnimalToAnimalGroups", x => x.MapAnimalToAnimalGroupId);
                    table.ForeignKey(
                        name: "FK_MapAnimalToAnimalGroups_AnimalGroups_AnimalGroupId",
                        column: x => x.AnimalGroupId,
                        principalTable: "AnimalGroups",
                        principalColumn: "AnimalGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapAnimalToAnimalGroups_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "AnimalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 27, "Create Animal Groups", "create_animal_groups" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 28, "Delete Animal Groups", "delete_animal_groups" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 29, "Edit Animal Groups", "edit_animal_groups" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 30, "View Animal Groups", "view_animal_groups" });

            migrationBuilder.CreateIndex(
                name: "IX_MapAnimalToAnimalGroups_AnimalId",
                table: "MapAnimalToAnimalGroups",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_MapAnimalToAnimalGroups_AnimalGroupId_AnimalId",
                table: "MapAnimalToAnimalGroups",
                columns: new[] { "AnimalGroupId", "AnimalId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapAnimalToAnimalGroups");

            migrationBuilder.DropTable(
                name: "AnimalGroups");

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 30);
        }
    }
}
