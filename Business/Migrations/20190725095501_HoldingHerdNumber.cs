using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class HoldingHerdNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HerdNumber",
                table: "MapHoldingRegistrationToHoldings",
                nullable: true,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE \"MapHoldingRegistrationToHoldings\" SET \"HerdNumber\" = 'N/A';"
            );

            migrationBuilder.AlterColumn<string>(
                name: "HerdNumber",
                table: "MapHoldingRegistrationToHoldings",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "EnumHoldingRegistrations",
                keyColumn: "EnumHoldingRegistrationId",
                keyValue: 5,
                column: "Description",
                value: "Sheep and Goats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HerdNumber",
                table: "MapHoldingRegistrationToHoldings");

            migrationBuilder.UpdateData(
                table: "EnumHoldingRegistrations",
                keyColumn: "EnumHoldingRegistrationId",
                keyValue: 5,
                column: "Description",
                value: "Sheeps and Goats");
        }
    }
}
