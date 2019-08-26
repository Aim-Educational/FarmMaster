using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Business.Migrations
{
    public partial class LifeEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataType",
                table: "AnimalCharacteristics");

            migrationBuilder.CreateTable(
                name: "LifeEvents",
                columns: table => new
                {
                    LifeEventId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 75, nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeEvents", x => x.LifeEventId);
                });

            migrationBuilder.CreateTable(
                name: "LifeEventDynamicFieldInfo",
                columns: table => new
                {
                    LifeEventDynamicFieldInfoId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 75, nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: true),
                    Type = table.Column<string>(nullable: false),
                    LifeEventId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeEventDynamicFieldInfo", x => x.LifeEventDynamicFieldInfoId);
                    table.ForeignKey(
                        name: "FK_LifeEventDynamicFieldInfo_LifeEvents_LifeEventId",
                        column: x => x.LifeEventId,
                        principalTable: "LifeEvents",
                        principalColumn: "LifeEventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LifeEventEntries",
                columns: table => new
                {
                    LifeEventEntryId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    LifeEventId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeEventEntries", x => x.LifeEventEntryId);
                    table.ForeignKey(
                        name: "FK_LifeEventEntries_LifeEvents_LifeEventId",
                        column: x => x.LifeEventId,
                        principalTable: "LifeEvents",
                        principalColumn: "LifeEventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LifeEventDynamicFieldValues",
                columns: table => new
                {
                    LifeEventDynamicFieldValueId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Value = table.Column<string>(nullable: false),
                    LifeEventDynamicFieldInfoId = table.Column<int>(nullable: false),
                    LifeEventEntryId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeEventDynamicFieldValues", x => x.LifeEventDynamicFieldValueId);
                    table.ForeignKey(
                        name: "FK_LifeEventDynamicFieldValues_LifeEventDynamicFieldInfo_LifeE~",
                        column: x => x.LifeEventDynamicFieldInfoId,
                        principalTable: "LifeEventDynamicFieldInfo",
                        principalColumn: "LifeEventDynamicFieldInfoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LifeEventDynamicFieldValues_LifeEventEntries_LifeEventEntry~",
                        column: x => x.LifeEventEntryId,
                        principalTable: "LifeEventEntries",
                        principalColumn: "LifeEventEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LifeEventDynamicFieldInfo_LifeEventId",
                table: "LifeEventDynamicFieldInfo",
                column: "LifeEventId");

            migrationBuilder.CreateIndex(
                name: "IX_LifeEventDynamicFieldInfo_Name_LifeEventId",
                table: "LifeEventDynamicFieldInfo",
                columns: new[] { "Name", "LifeEventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LifeEventDynamicFieldValues_LifeEventEntryId",
                table: "LifeEventDynamicFieldValues",
                column: "LifeEventEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_LifeEventDynamicFieldValues_LifeEventDynamicFieldInfoId_Lif~",
                table: "LifeEventDynamicFieldValues",
                columns: new[] { "LifeEventDynamicFieldInfoId", "LifeEventEntryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LifeEventEntries_LifeEventId",
                table: "LifeEventEntries",
                column: "LifeEventId");

            migrationBuilder.CreateIndex(
                name: "IX_LifeEvents_Name",
                table: "LifeEvents",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LifeEventDynamicFieldValues");

            migrationBuilder.DropTable(
                name: "LifeEventDynamicFieldInfo");

            migrationBuilder.DropTable(
                name: "LifeEventEntries");

            migrationBuilder.DropTable(
                name: "LifeEvents");

            migrationBuilder.AddColumn<string>(
                name: "DataType",
                table: "AnimalCharacteristics",
                nullable: false,
                defaultValue: "");
        }
    }
}
