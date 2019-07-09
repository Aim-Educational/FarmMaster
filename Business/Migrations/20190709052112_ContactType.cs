using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class ContactType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactType",
                table: "Contacts",
                nullable: true,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE \"Contacts\" SET \"ContactType\" = 'User';"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactType",
                table: "Contacts");
        }
    }
}
