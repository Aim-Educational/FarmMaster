using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class UserEmailVerify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationToken",
                table: "UserPrivacy",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerificationToken",
                table: "UserPrivacy");
        }
    }
}
