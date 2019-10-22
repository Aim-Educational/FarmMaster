using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class MoreRequestMetrics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "MetricRequests");

            migrationBuilder.AddColumn<string>(
                name: "Ip",
                table: "MetricRequests",
                maxLength: 45,
                nullable: false,
                defaultValue: "N/A");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "MetricRequests",
                maxLength: 100,
                nullable: false,
                defaultValue: "N/A");

            migrationBuilder.AddColumn<string>(
                name: "TraceIdentifier",
                table: "MetricRequests",
                maxLength: 25,
                nullable: false,
                defaultValue: "N/A");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ip",
                table: "MetricRequests");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "MetricRequests");

            migrationBuilder.DropColumn(
                name: "TraceIdentifier",
                table: "MetricRequests");

            migrationBuilder.AddColumn<long>(
                name: "Count",
                table: "MetricRequests",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
