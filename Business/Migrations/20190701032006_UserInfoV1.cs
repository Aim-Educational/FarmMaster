using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class UserInfoV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PassHash",
                table: "UserLoginInfo",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldMaxLength: 60);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "UserLoginInfo",
                maxLength: 75,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "MiddleNames",
                table: "Contacts",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "UserLoginInfo");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PassHash",
                table: "UserLoginInfo",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "MiddleNames",
                table: "Contacts",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);
        }
    }
}
