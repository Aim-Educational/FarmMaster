using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class RoleDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Roles",
                maxLength: 150,
                nullable: false,
                defaultValue: "N/A");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Roles");
        }
    }
}
