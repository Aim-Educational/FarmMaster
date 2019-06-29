using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    MiddleNames = table.Column<string>(maxLength: 150, nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ContactId);
                });

            migrationBuilder.CreateTable(
                name: "EnumRolePermissions",
                columns: table => new
                {
                    EnumRolePermissionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InternalName = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 75, nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnumRolePermissions", x => x.EnumRolePermissionId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    ParentRoleId = table.Column<int>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Roles_Roles_ParentRoleId",
                        column: x => x.ParentRoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginInfo",
                columns: table => new
                {
                    UserLoginInfoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PassHash = table.Column<byte[]>(maxLength: 60, nullable: false),
                    Salt = table.Column<string>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginInfo", x => x.UserLoginInfoId);
                });

            migrationBuilder.CreateTable(
                name: "Telephones",
                columns: table => new
                {
                    TelephoneId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Number = table.Column<string>(maxLength: 15, nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telephones", x => x.TelephoneId);
                    table.ForeignKey(
                        name: "FK_Telephones_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MapRolePermissionToRoles",
                columns: table => new
                {
                    MapRolePermissionToRoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    EnumRolePermissionId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapRolePermissionToRoles", x => x.MapRolePermissionToRoleId);
                    table.ForeignKey(
                        name: "FK_MapRolePermissionToRoles_EnumRolePermissions_EnumRolePermissionId",
                        column: x => x.EnumRolePermissionId,
                        principalTable: "EnumRolePermissions",
                        principalColumn: "EnumRolePermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapRolePermissionToRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContactId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: true),
                    UserLoginInfoId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_UserLoginInfo_UserLoginInfoId",
                        column: x => x.UserLoginInfoId,
                        principalTable: "UserLoginInfo",
                        principalColumn: "UserLoginInfoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 1, "Edit Contacts", "edit_contacts" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 2, "View Contacts", "view_contacts" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 3, "Edit Roles", "edit_roles" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 4, "View Roles", "view_roles" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 5, "Edit Users", "edit_users" });

            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 6, "View Users", "view_users" });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Email",
                table: "Contacts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnumRolePermissions_InternalName",
                table: "EnumRolePermissions",
                column: "InternalName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MapRolePermissionToRoles_RoleId",
                table: "MapRolePermissionToRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MapRolePermissionToRoles_EnumRolePermissionId_RoleId",
                table: "MapRolePermissionToRoles",
                columns: new[] { "EnumRolePermissionId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ParentRoleId",
                table: "Roles",
                column: "ParentRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Telephones_ContactId",
                table: "Telephones",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ContactId",
                table: "Users",
                column: "ContactId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserLoginInfoId",
                table: "Users",
                column: "UserLoginInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapRolePermissionToRoles");

            migrationBuilder.DropTable(
                name: "Telephones");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "EnumRolePermissions");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "UserLoginInfo");
        }
    }
}
