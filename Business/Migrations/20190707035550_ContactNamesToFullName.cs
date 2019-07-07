using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class ContactNamesToFullName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Contacts",
                nullable: true,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE \"Contacts\" SET \"FullName\" = \"FirstName\" || ' ' || \"MiddleNames\" || ' ' || \"LastName\";"
            );

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "MiddleNames",
                table: "Contacts");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Contacts",
                nullable: true,
                defaultValue: "",
                oldNullable: false
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Contacts");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Contacts",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Contacts",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleNames",
                table: "Contacts",
                maxLength: 150,
                nullable: true);
        }
    }
}
