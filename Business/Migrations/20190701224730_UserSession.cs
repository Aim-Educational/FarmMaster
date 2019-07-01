using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class UserSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserPrivacyId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "SessionToken",
                table: "UserLoginInfo",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SessionTokenExpiry",
                table: "UserLoginInfo",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserPrivacyId",
                table: "Users",
                column: "UserPrivacyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserPrivacyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SessionToken",
                table: "UserLoginInfo");

            migrationBuilder.DropColumn(
                name: "SessionTokenExpiry",
                table: "UserLoginInfo");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserPrivacyId",
                table: "Users",
                column: "UserPrivacyId");
        }
    }
}
