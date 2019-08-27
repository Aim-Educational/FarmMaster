using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class LifeEventStuff1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBuiltin",
                table: "LifeEvents",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_IsAnonymous",
                table: "Contacts",
                column: "IsAnonymous");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contacts_IsAnonymous",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "IsBuiltin",
                table: "LifeEvents");
        }
    }
}
