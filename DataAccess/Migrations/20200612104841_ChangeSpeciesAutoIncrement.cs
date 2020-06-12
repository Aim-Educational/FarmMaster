using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ChangeSpeciesAutoIncrement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if(!migrationBuilder.ActiveProvider.Contains("Postgre"))
                return;
            
            // Because of that super weird bug, we have to seed the first species at 1.
            // The autoincrement doesn't bump however, so we'll do that ourself.
            migrationBuilder.Sql("ALTER SEQUENCE \"Species_SpeciesId_seq\" RESTART WITH 2;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
