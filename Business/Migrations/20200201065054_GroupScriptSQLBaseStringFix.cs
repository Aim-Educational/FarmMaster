using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class GroupScriptSQLBaseStringFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // "INNER" -> "LEFT"
            // DISTINCT by default to combat the way "LEFT" is affecting the query.
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION public._sp_animalgroupscriptbasequerystring(
	)
    RETURNS text
    LANGUAGE 'plpgsql'

    COST 100
    IMMUTABLE 
AS $BODY$
BEGIN
	RETURN 
	'SELECT DISTINCT ""Animals"".*
		FROM ""Animals""
		LEFT JOIN ""MapLifeEventEntryToAnimals""
			ON ""MapLifeEventEntryToAnimals"".""AnimalId"" = ""Animals"".""AnimalId""
			LEFT JOIN ""LifeEventEntries""
				ON ""LifeEventEntries"".""LifeEventEntryId"" = ""MapLifeEventEntryToAnimals"".""LifeEventEntryId""
				LEFT JOIN ""LifeEvents""
					ON ""LifeEvents"".""LifeEventId"" = ""LifeEventEntries"".""LifeEventId""
				LEFT JOIN ""LifeEventDynamicFieldValues"" 
					ON ""LifeEventDynamicFieldValues"".""LifeEventEntryId"" = ""LifeEventEntries"".""LifeEventEntryId""
					LEFT JOIN ""LifeEventDynamicFieldInfo""
						ON ""LifeEventDynamicFieldInfo"".""LifeEventDynamicFieldInfoId"" = ""LifeEventDynamicFieldValues"".""LifeEventDynamicFieldInfoId""
		LEFT JOIN ""MapBreedToAnimals""
			ON ""MapBreedToAnimals"".""AnimalId"" = ""Animals"".""AnimalId""
	WHERE';
END
$BODY$;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
