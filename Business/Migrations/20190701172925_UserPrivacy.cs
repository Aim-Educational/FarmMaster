using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class UserPrivacy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserPrivacyId",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserPrivacy",
                columns: table => new
                {
                    UserPrivacyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HasVerifiedEmail = table.Column<bool>(nullable: false),
                    TermsOfServiceVersionAgreedTo = table.Column<int>(nullable: false),
                    PrivacyPolicyVersionAgreedTo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrivacy", x => x.UserPrivacyId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserPrivacyId",
                table: "Users",
                column: "UserPrivacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserPrivacy_UserPrivacyId",
                table: "Users",
                column: "UserPrivacyId",
                principalTable: "UserPrivacy",
                principalColumn: "UserPrivacyId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserPrivacy_UserPrivacyId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserPrivacy");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserPrivacyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserPrivacyId",
                table: "Users");
        }
    }
}
