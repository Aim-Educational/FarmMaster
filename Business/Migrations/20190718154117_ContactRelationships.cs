using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    public partial class ContactRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MapContactRelationships",
                columns: table => new
                {
                    MapContactRelationshipId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ContactOneId = table.Column<int>(nullable: false),
                    ContactTwoId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapContactRelationships", x => x.MapContactRelationshipId);
                    table.ForeignKey(
                        name: "FK_MapContactRelationships_Contacts_ContactOneId",
                        column: x => x.ContactOneId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapContactRelationships_Contacts_ContactTwoId",
                        column: x => x.ContactTwoId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapContactRelationships_ContactOneId",
                table: "MapContactRelationships",
                column: "ContactOneId");

            migrationBuilder.CreateIndex(
                name: "IX_MapContactRelationships_ContactTwoId",
                table: "MapContactRelationships",
                column: "ContactTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_MapContactRelationships_ContactOneId_ContactTwoId",
                table: "MapContactRelationships",
                columns: new[] { "ContactOneId", "ContactTwoId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapContactRelationships");
        }
    }
}
