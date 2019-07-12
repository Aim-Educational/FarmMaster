using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    public partial class ActionsAgainstContactInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionsAgainstContactInfo",
                columns: table => new
                {
                    ActionAgainstContactInfoId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UserResponsibleId = table.Column<int>(nullable: false),
                    ContactAffectedId = table.Column<int>(nullable: false),
                    ActionType = table.Column<string>(nullable: false),
                    Reason = table.Column<string>(maxLength: 150, nullable: false),
                    AdditionalInfo = table.Column<string>(maxLength: 150, nullable: true),
                    DateTimeUtc = table.Column<DateTimeOffset>(nullable: false),
                    HasContactBeenInformed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionsAgainstContactInfo", x => x.ActionAgainstContactInfoId);
                    table.ForeignKey(
                        name: "FK_ActionsAgainstContactInfo_Contacts_ContactAffectedId",
                        column: x => x.ContactAffectedId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionsAgainstContactInfo_Users_UserResponsibleId",
                        column: x => x.UserResponsibleId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionsAgainstContactInfo_ContactAffectedId",
                table: "ActionsAgainstContactInfo",
                column: "ContactAffectedId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionsAgainstContactInfo_HasContactBeenInformed",
                table: "ActionsAgainstContactInfo",
                column: "HasContactBeenInformed");

            migrationBuilder.CreateIndex(
                name: "IX_ActionsAgainstContactInfo_UserResponsibleId",
                table: "ActionsAgainstContactInfo",
                column: "UserResponsibleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionsAgainstContactInfo");
        }
    }
}
