using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class ContactRelationTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "MapContactRelationships",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "MapContactRelationships");
        }
    }
}
