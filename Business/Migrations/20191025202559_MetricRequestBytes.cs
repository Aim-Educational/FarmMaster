using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class MetricRequestBytes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BytesUsedAtEnd",
                table: "MetricRequests",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BytesUsedAtStart",
                table: "MetricRequests",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BytesUsedAtEnd",
                table: "MetricRequests");

            migrationBuilder.DropColumn(
                name: "BytesUsedAtStart",
                table: "MetricRequests");
        }
    }
}
